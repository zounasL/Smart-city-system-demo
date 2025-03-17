using Esri.ArcGISMapsSDK.Components;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class IoTCameraLowEndService : MonoBehaviour
{
    public string blobStorageToken = string.Empty;
    public string blobEndpoint = string.Empty;
    public string iotDeviceId = string.Empty;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(gameObject.name + " | Colission");

        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 3);
        // NOTE: file name is a number!
        string path = "Assets/Resources/vehicles/" + randomNumber + ".jpg";
        StartCoroutine(UploadImageToBlobStorage(path));
    }

    private IEnumerator UploadImageToBlobStorage(string imagePath)
    {
        // Read image bytes (you can load the image from Resources, StreamingAssets, etc.)
        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

        ArcGISLocationComponent arcGISLocation = FindObjectOfType<ArcGISLocationComponent>();
        string coordinatesOfTheIoTDevice = $"{arcGISLocation.Position.Y},{arcGISLocation.Position.X}";

        // Upload image to blob storage
        UnityWebRequest request = UnityWebRequest.Put($"{blobEndpoint}/{iotDeviceId}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg?{blobStorageToken}", imageBytes);
        request.SetRequestHeader("x-ms-blob-type", "BlockBlob"); 
        request.SetRequestHeader("x-ms-meta-device", iotDeviceId);
        request.SetRequestHeader("x-ms-meta-coordinates", coordinatesOfTheIoTDevice);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image uploaded successfully!");
        }
        else
        {
            Debug.LogError($"Error uploading image: {request.error}");
        }
    }
}
