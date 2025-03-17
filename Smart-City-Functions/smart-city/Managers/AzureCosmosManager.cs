using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace smart_city.Managers
{
    class AzureCosmosManager
    {
        public CosmosClient cosmosClient;
        public Database database;
        public Container container;
        public AzureCosmosManager(string connection)
        {
            CosmosClientOptions cosmosClientOptions = new()
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            };

            var connectionDictionary = new Dictionary<string, string>();
            foreach (var item in connection.Split(';'))
            {
                var itemKeyValue = item.Split('=', 2);
                connectionDictionary.Add(itemKeyValue[0], itemKeyValue[1]);
            }

            string a = connectionDictionary["AccountKey"];
            cosmosClient = new(
                accountEndpoint: connectionDictionary["AccountEndpoint"],
                authKeyOrResourceToken: connectionDictionary["AccountKey"],
                clientOptions: cosmosClientOptions
            );
        }

        public async Task InitAsync(string databaseId, string containerId, string partitionId)
        {
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(
               id: databaseId,
               throughput: 400
           );

            container = await database.CreateContainerIfNotExistsAsync(
                id: containerId,
                partitionKeyPath: partitionId
            );
        } 
    }
}
