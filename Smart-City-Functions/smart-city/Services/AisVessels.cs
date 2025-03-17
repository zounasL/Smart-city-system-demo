using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO.Compression;
using smart_city.Models;
using System.Linq;

namespace smart_city.Services
{
    public static class AisVessels
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string aisEndpoint = Environment.GetEnvironmentVariable("AisEndpoint");

        [FunctionName("AisVessels")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("AIS vessels data fetch");

            try
            {
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                HttpResponseMessage response = await httpClient.GetAsync(aisEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    using var decompressedStream = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
                    using var reader = new StreamReader(decompressedStream);
                    var decompressedContent = await reader.ReadToEndAsync();


                    var aisData = JsonConvert.DeserializeObject<AISModel>(decompressedContent);
                    return new OkObjectResult(aisData.features.Where(vessel => !vessel.properties.IsTimestampOlderThanDays(3)).Take(300));
                }
                else
                {
                    log.LogError($"API call failed with status code: {response.StatusCode}");
                    return new BadRequestObjectResult("API call failed");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error while making API call: {ex.Message}");
                return new BadRequestObjectResult("Error occurred");
            }
        }
    }
}
