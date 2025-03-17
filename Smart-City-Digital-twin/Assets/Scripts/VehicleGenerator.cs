using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour
{
    public string azureFunctionToken = string.Empty;
    public string azureFunctionsUrl = string.Empty;
    public GameObject vehiclePrefab;

    private AzureFunctionsService azureFunctionsService;

    // Start is called before the first frame update
    void Start()
    {
        // Init azure service
        azureFunctionsService = new AzureFunctionsService(azureFunctionToken, azureFunctionsUrl);
        // Generate vehicles into the game world
        generateVehicles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void generateVehicles()
    {
        // Fetch vehicles data from Azure
        StartCoroutine(azureFunctionsService.SensorsVehicleData((vehicles =>
        {
            // Loop over the data and fabricate objects
            foreach (var vehicle in vehicles)
            {
                // Store vehicle path locations
                List<double[]> vehiclePath = new List<double[]>(vehicle);
                // Create a new vehicle object
                GameObject vehiclefab = Instantiate(vehiclePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                // Attach ArcGIS location component to the object
                ArcGISLocationComponent vehicleLocation = vehiclefab.AddComponent<ArcGISLocationComponent>();
                // Set default location
                vehicleLocation.Position = new ArcGISPoint(0, 0, 0, ArcGISSpatialReference.WGS84());
                // Get router handler script
                VehicleRouteHandler vehicleRouteHandler = vehiclefab.gameObject.GetComponent<VehicleRouteHandler>();
                // Set path
                vehicleRouteHandler.path = vehiclePath;
                // Add object into the game
                vehiclefab.transform.SetParent(gameObject.transform);
                // Enable ArcGIS location component
                vehicleLocation.enabled = true;
                // Notify that init is completed
                vehicleRouteHandler.initialized = true;
            }
        })));
    }
}
