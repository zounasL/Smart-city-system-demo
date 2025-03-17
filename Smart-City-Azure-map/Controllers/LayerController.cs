using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Smart_City_Azure_map.Models;
using Smart_City_Azure_map.Services;

namespace Smart_City_Azure_map.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerController : ControllerBase
    {
        private BlobStorageService blobStorageService;
        public LayerController(IConfiguration configuration) {
            blobStorageService = new BlobStorageService(configuration);
        }

        // GET: api/layer
        [HttpGet]
        async public Task<JsonResult> Get()
        {
            System.Diagnostics.Debug.WriteLine("Listing blobs...");

            List<string> blobs = await blobStorageService.getLayersList("layers");
            return new JsonResult(blobs.ToArray());
        }

        // GET: api/layer/:layerName
        [HttpGet("heatmap/{layerName}")]
        async public Task<JsonResult> GetHeatmap(string layerName)
        {
            BlobClient blob = blobStorageService.getBlobClient("layers", layerName);
            Stream fileStream = await blob.OpenReadAsync();

            HeatmapLayerModel layer;
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var sr = new StreamReader(fileStream))
            using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(sr))
            {
                layer = serializer.Deserialize<HeatmapLayerModel>(jsonTextReader);
            }

            return new JsonResult(layer);
        }

        [HttpGet("traffic/{layerName}")]
        async public Task<JsonResult> GetTraffic(string layerName)
        {
            BlobClient blob = blobStorageService.getBlobClient("layers", layerName);
            Stream fileStream = await blob.OpenReadAsync();

            TrafficLayerModel[] layer;
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var sr = new StreamReader(fileStream))
            using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(sr))
            {
                layer = serializer.Deserialize<TrafficLayerModel[]>(jsonTextReader);
            }

            return new JsonResult(layer);
        }
    }
}
