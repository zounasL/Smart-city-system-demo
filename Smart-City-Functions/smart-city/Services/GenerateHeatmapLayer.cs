using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using smart_city.Managers;
using Microsoft.Azure.Cosmos;
using smart_city.Models;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace smart_city.Services
{
    public static class GenerateHeatmapLayer
    {
        [FunctionName("GenerateHeatmapLayer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Blob("layers")] BlobContainerClient blobContainerClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            AzureCosmosManager azureCosmosManager = new AzureCosmosManager(Environment.GetEnvironmentVariable("AzureCosmosConnection"));
            await azureCosmosManager.InitAsync("IoT", "trafficEvents", "/deviceId");

            DateTime startingTime;
            // xx.xx.xxxx yy:yy
            if (!DateTime.TryParseExact(HttpUtility.UrlDecode(req.Query["startingTime"]), "dd.MM.yyyy HH:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out startingTime)) {
                return new BadRequestObjectResult("");
            }
            DateTime endingTime;
            // xx.xx.xxxx yy:yy
            if (!DateTime.TryParseExact(HttpUtility.UrlDecode(req.Query["endingTime"]), "dd.MM.yyyy HH:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out endingTime))
            {
                return new BadRequestObjectResult("");
            }


            // Build query definition
            var parameterizedQuery = new QueryDefinition(
                query: "SELECT* FROM trafficEvents WHERE trafficEvents.timestamp BETWEEN @starting AND @ending"
            )
                .WithParameter("@starting", startingTime.ToString("o", CultureInfo.InvariantCulture))
                .WithParameter("@ending", endingTime.ToString("o", CultureInfo.InvariantCulture));

            // Query multiple items from container
            using FeedIterator<IoTEventCosmosModel> filteredFeed = azureCosmosManager.container.GetItemQueryIterator<IoTEventCosmosModel>(
                queryDefinition: parameterizedQuery
            );

            List<AzureMapHeatmapLayerFeature> features = new List<AzureMapHeatmapLayerFeature> ();

            // Iterate query result pages
            while (filteredFeed.HasMoreResults)
            {
                FeedResponse<IoTEventCosmosModel> response = await filteredFeed.ReadNextAsync();
                foreach (IoTEventCosmosModel item in response)
                {
                    double[] coordinatesArray = item.location.Position.Coordinates.ToArray();
                    Array.Reverse(coordinatesArray);
                    features.Add(new AzureMapHeatmapLayerFeature
                    {
                        id = item.id,
                        geometry = new AzureMapHeatmapLayerPoint
                        {
                            coordinates = coordinatesArray
                        }
                    });
                }
            }

            if(features.Count == 0)
            {
                return new BadRequestObjectResult("No records found");
            }

            AzureMapHeatmapLayer heatmap = new AzureMapHeatmapLayer
            {
                features = features.ToArray()
            };

            blobContainerClient.CreateIfNotExists();
            BlobClient blob = blobContainerClient.GetBlobClient($"heatmap_{startingTime.ToString("dd-MM-yyyy-HH-ss")}_{endingTime.ToString("dd-MM-yyyy-HH-ss")}.geojson");

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(heatmap))))
            {
                await blob.UploadAsync(ms);
            }

            return new OkObjectResult("Heatmap layer created");
        }
    }
}
