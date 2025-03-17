using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;

namespace smart_city.Services
{
    public static class SensorsVehicleData
    {
        [FunctionName("SensorsVehicleData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Blob("iot-sensors")] BlobContainerClient blobContainerClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            log.LogInformation("Blobs within container:");
            BlobClient blob = blobContainerClient.GetBlobClient("data.json");
            Stream fileStream = await blob.OpenReadAsync();

            var serializer = new JsonSerializer();

            List<double[][]> coordinates = new List<double[][]>();
            using (var sr = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                VehivleRoute[] vehivleRoutes = serializer.Deserialize<VehivleRoute[]>(jsonTextReader);
                foreach (var route in vehivleRoutes)
                {
                    coordinates.Add(route.path);
                }
            }

            return new OkObjectResult(coordinates.ToArray());
        }
    }

    [Serializable]
    public class VehivleRoute
    {
        public string vendor { get; set; }
        public double[][] path { get; set; }
        public int[] timestamps { get; set; }
}
}
