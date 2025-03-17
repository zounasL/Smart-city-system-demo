using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Collections;

namespace smart_city.Services
{
    public static class ObjectDetectionCustomVision
    {
        private static string predictionEndpoint = Environment.GetEnvironmentVariable("VISION_PREDICTION_ENDPOINT");
        private static string predictionKey = Environment.GetEnvironmentVariable("VISION_PREDICTION_KEY");
        private static string publishedModelName = Environment.GetEnvironmentVariable("VISION_MODEL");
        private static string projectId = Environment.GetEnvironmentVariable("VISION_PROJECT_ID");

        [FunctionName("ObjectDetectionCustomVision")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            // Get the image file from the request
            var file = req.Form.Files["image"];

            // Check if file exists
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("Please upload an image file.");
            }

            CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);
            dynamic predictionResponse = null;

            Console.WriteLine("Making a prediction:");
            using (var stream = file.OpenReadStream())
            {
                var result = predictionApi.DetectImage(Guid.Parse(projectId), publishedModelName, stream);

                predictionResponse = JsonConvert.SerializeObject(result.Predictions.Where(x => x.Probability > 0.75), Formatting.Indented);

                // Loop over each prediction and write out the results
                foreach (var c in result.Predictions)
                {
                    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1} [ {c.BoundingBox.Left}, {c.BoundingBox.Top}, {c.BoundingBox.Width}, {c.BoundingBox.Height} ]");
                }

                // result.Predictions.MaxBy(x => x.Probability)
            }

            return new OkObjectResult(predictionResponse);
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
