using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AzureFunctionsService
{
    protected string token;
    protected string endpointUrl = "";
    public AzureFunctionsService(string _token, string _endpoint)
    {
        if(_endpoint == null)
        {
            Debug.LogError("Azure function endpoint missing!");
        }
        token = _token;
        endpointUrl = _endpoint;
    }

    public IEnumerator CloudImageDetection(byte[] imageData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "iotImage.jpg");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(endpointUrl + "/ObjectDetectionCustomVision", form))
        {
            webRequest.SetRequestHeader("Authorization", token);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator SensorsVehicleData(Action<double[][][]> callback)
    {
        // Get vehicle data
        using (UnityWebRequest webRequest = UnityWebRequest.Get(endpointUrl + "/SensorsVehicleData"))
        {
            // Add authorization token
            webRequest.SetRequestHeader("Authorization", token);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Parse data
                double[][][] vehicles = JsonConvert.DeserializeObject<double[][][]>(webRequest.downloadHandler.text);
                // Return data
                callback(vehicles);
            }
        }
    }

    public IEnumerator AISVesselseData(Action<MarineVesselModel[]> callback)
    {
        // Get vehicle data
        using (UnityWebRequest webRequest = UnityWebRequest.Get(endpointUrl + "/AisVessels"))
        {
            // Add authorization token
            webRequest.SetRequestHeader("Authorization", token);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                var jsonResponse = webRequest.downloadHandler.text;

                bool threadFinished = false;
                MarineVesselModel[] vessels = null;
                Thread t = new Thread(delegate () {
                    Debug.Log("Deserializating data");
                    vessels = JsonConvert.DeserializeObject<MarineVesselModel[]>(jsonResponse);
                    Debug.Log("Data deserialization done");
                    threadFinished = true;
                });
                t.Start();
                while (!threadFinished)
                {
                    Debug.Log("Waiting for thread to deserialize vessels");
                    yield return null;
                }

                callback(vessels.ToArray());
            }
        }
    }

    public class MarineVesselModel
    {
        public int mmsi { get; set; }
        public VesselCoodinate geometry { get; set; }
        public VesselPropesties properties { get; set; }
    }

    public class VesselCoodinate
    {
        public string type { get; set; } = "Point";
        public double[] coordinates { get; set; }
    }

    public class VesselPropesties
    {
        public float sog { get; set; }
        public float cog { get; set; }
        public float rot { get; set; }
        public float heading { get; set; }
    }
}
