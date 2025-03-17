using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using smart_city.Managers;
using smart_city.Models;
using Azure.Messaging.EventHubs;
using System.Text;
using System.Globalization;

namespace smart_city.Triggers
{
    public class IoTDeviceMessageHandlerIoTHub
    {
        private AzureCosmosManager azureCosmosManager = new AzureCosmosManager(Environment.GetEnvironmentVariable("AzureCosmosConnection"));
        [FunctionName("IoTDeviceMessageHandlerIoTHub")]
        public async Task RunAsync(
            [IoTHubTrigger("messages/events", Connection = "AzureIoTHubConnection")] EventData message,
            ILogger log
            )
        {
            string messageData = Uri.UnescapeDataString(Encoding.UTF8.GetString(message.Body.ToArray()));
            log.LogInformation($"IoT Hub trigger function processed a message: {messageData}");

            IoTEventMessageModel iotEvent = JsonConvert.DeserializeObject<IoTEventMessageModel>(messageData);

            // %7b%22deviceId%22%3a%22IoT-virtual-unity-device%22%2c%22coordinates%22%3a%22%5b62.24147%2c%2025.72088%5d%22%7d
            IoTEventCosmosModel iotCosmosEventDocument = iotEvent.getCosmosDocumentModel();

            await azureCosmosManager.InitAsync("IoT", "trafficEvents", "/deviceId");
            await azureCosmosManager.container.UpsertItemAsync(iotCosmosEventDocument);

            log.LogInformation("Feature added to Cosmos DB");
        }
    }
}
