using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using smart_city.Managers;
using smart_city.Models;

namespace smart_city.Triggers
{
    public class IoTDeviceBlobHandler
    {
        private static string predictionEndpoint = Environment.GetEnvironmentVariable("VISION_PREDICTION_ENDPOINT");
        private static string predictionKey = Environment.GetEnvironmentVariable("VISION_PREDICTION_KEY");
        private static string publishedModelName = Environment.GetEnvironmentVariable("VISION_MODEL");
        private static string projectId = Environment.GetEnvironmentVariable("VISION_PROJECT_ID");
        private AzureCosmosManager azureCosmosManager = new AzureCosmosManager(Environment.GetEnvironmentVariable("AzureCosmosConnection"));

        [FunctionName("IoTDeviceBlobHandler")]
        public async Task Run([BlobTrigger("iot-images/{name}", Connection = "AzureWebJobsStorage")]Stream imageBlob, string name, ILogger log)
        {

            CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);
            var result = predictionApi.DetectImage(Guid.Parse(projectId), publishedModelName, imageBlob);

            int predictionResponse = result.Predictions.Where(x => x.Probability > 0.98).Count();

            // Loop over each prediction and write out the results
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"\t{c.TagName}: {c.Probability:P1} [ {c.BoundingBox.Left}, {c.BoundingBox.Top}, {c.BoundingBox.Width}, {c.BoundingBox.Height} ]");
            }


            // Retrieve the blob reference
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("iot-images");
            var blob = container.GetBlockBlobReference(name);

            // Fetch metadata
            await blob.FetchAttributesAsync();

            // Access metadata properties
            var deviceId = blob.Metadata.ContainsKey("device") ? blob.Metadata["device"] : null;
            var coordinates = blob.Metadata.ContainsKey("coordinates") ? blob.Metadata["coordinates"] : null;

            if(coordinates != null && deviceId != null)
            {
                IoTEventMessageModel iotEvent = new IoTEventMessageModel();
                iotEvent.deviceId = deviceId;
                iotEvent.coordinates = coordinates;
                iotEvent.data = JsonConvert.SerializeObject(new { vehicles = predictionResponse });

                IoTEventCosmosModel iotCosmosEventDocument = iotEvent.getCosmosDocumentModel();

                await azureCosmosManager.InitAsync("IoT", "trafficEvents", "/deviceId");
                await azureCosmosManager.container.UpsertItemAsync(iotCosmosEventDocument);

                log.LogInformation("Feature added to Cosmos DB");
            }
        }

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }
    }
}