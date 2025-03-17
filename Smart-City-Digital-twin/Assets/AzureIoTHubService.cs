
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

public class AzureIoTHubService
{
    public string deviceID;
    public string hubName;
    protected string token;
    protected string azureIoTHubEndpoint;

    public AzureIoTHubService(string _deviceID, string _hubName, string _token)
    {
        deviceID = _deviceID;
        hubName = _hubName;
        token = _token;
        azureIoTHubEndpoint = String.Format("https://{0}.azure-devices.net/devices/{1}/messages/events?api-version=2020-03-13", hubName, deviceID);
    }

    public IEnumerator sendMessage(string data, string coodrinates)
    {
        Debug.Log(String.Format("Device {0}; Sending message", deviceID));

        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("deviceId", deviceID);
        body.Add("coordinates", coodrinates);
        body.Add("data", data);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(azureIoTHubEndpoint, JsonConvert.SerializeObject(body)))
        {
            webRequest.SetRequestHeader("Authorization", token);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }
}
