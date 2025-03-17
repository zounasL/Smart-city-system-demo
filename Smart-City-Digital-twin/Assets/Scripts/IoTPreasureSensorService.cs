using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

public class IoTPreasureSensorService : MonoBehaviour
{
    public string iotDeviceId = string.Empty;
    public string iotHubName = string.Empty;
    public string iotHubToken = string.Empty;

    private AzureIoTHubService azureIoTHubService;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Ray ray = new Ray(gameObject.transform.localPosition, Vector3.up * 50);
        RaycastHit hit;

        Debug.DrawRay(gameObject.transform.localPosition, Vector3.up * 50, Color.green);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
            Debug.Log("Raycasting: " + hit.point.x + " | " + hit.point.y + " | " + hit.point.z);
        }
        */
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(gameObject.name + " | Colission | Preasure sensor");
        // StartCoroutine(azureIoTHubService.sendMessage());
        /*
        string path = "Assets/Resources/test.jpg";
        byte[] imageBytes = File.ReadAllBytes(path);
        StartCoroutine(azureFunctionsService.cloudImageDetection(imageBytes));
        */
    }
}
