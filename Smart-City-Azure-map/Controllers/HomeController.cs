using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smart_City_Azure_map.Models;
using Smart_City_Azure_map.Services;
using System.Diagnostics;

namespace Smart_City_Azure_map.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BlobStorageService blobStorageService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            blobStorageService = new BlobStorageService(configuration);
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            ViewData["layersList"] = await getLayersList();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        async private Task<string[]> getLayersList()
        {
            List<string> layers = await blobStorageService.getLayersList("layers");
            return layers.ToArray();
        } 
    }
}