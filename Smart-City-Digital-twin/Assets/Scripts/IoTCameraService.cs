using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Esri.ArcGISMapsSDK.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class IoTCameraService : MonoBehaviour
{
    public string azureFunctionToken = string.Empty;
    public string azureFunctionsUrl = string.Empty;
    public string iotDeviceId = string.Empty;
    public string iotHubName = string.Empty;
    public string iotHubToken = string.Empty;

    private AzureIoTHubService azureIoTHubService;

    // Start is called before the first frame update
    void Start()
    {
        azureIoTHubService = new AzureIoTHubService(iotDeviceId, iotHubName, iotHubToken);
    }

    // Update is called once per frame
    void Update()
    {}

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(gameObject.name + " | Colission");
        string vehicleData = JsonConvert.SerializeObject(new {
            vehicles = 1
        });
        ArcGISLocationComponent arcGISLocation = FindObjectOfType<ArcGISLocationComponent>();
        
        string coordinatesOfTheIoTDevice = $"{arcGISLocation.Position.Y},{arcGISLocation.Position.X}";
        StartCoroutine(azureIoTHubService.sendMessage(vehicleData, coordinatesOfTheIoTDevice));
    }
}
