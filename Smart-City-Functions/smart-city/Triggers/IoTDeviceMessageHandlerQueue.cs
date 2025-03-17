using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using smart_city.Managers;
using smart_city.Models;

namespace smart_city.Triggers
{
    public class IoTDeviceMessageHandlerQueue
    {
        private AzureCosmosManager azureCosmosManager = new AzureCosmosManager(Environment.GetEnvironmentVariable("AzureCosmosConnection"));
        [FunctionName("IoTDeviceMessageHandlerQueue")]
        public async Task RunAsync([QueueTrigger("iot-device-message-events", Connection = "ioTDeviceMessageEventConnection")] string queueDataItem, ILogger log)
        {
            string messageData = queueDataItem;
            log.LogInformation($"Event message message: {messageData}");

            IoTEventMessageModel iotEvent = JsonConvert.DeserializeObject<IoTEventMessageModel>(messageData);
            IoTEventCosmosModel iotCosmosEventDocument = iotEvent.getCosmosDocumentModel();

            await azureCosmosManager.InitAsync("IoT", "trafficEvents", "/deviceId");
            await azureCosmosManager.container.UpsertItemAsync(iotCosmosEventDocument);

            log.LogInformation("Feature added to Cosmos DB");
        }
    }
}
