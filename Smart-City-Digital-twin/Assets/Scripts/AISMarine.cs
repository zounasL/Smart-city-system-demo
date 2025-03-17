using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static AzureFunctionsService;

public class AISMarine : MonoBehaviour
{
    public string azureFunctionToken = string.Empty;
    public string azureFunctionsUrl = string.Empty;
    public GameObject vehiclePrefab;

    private AzureFunctionsService azureFunctionsService;
    private float nextActionTime = 0.0f;
    public float period = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Init azure service
        azureFunctionsService = new AzureFunctionsService(azureFunctionToken, azureFunctionsUrl);
        StartCoroutine(NetworkCallCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void generateVessels()
    {
        // Fetch vehicles data from Azure
        StartCoroutine(azureFunctionsService.AISVesselseData((vessels =>
        {
            StartCoroutine(UpdateGameObjects(vessels));
        })));
    }

    private IEnumerator NetworkCallCoroutine()
    {
        while (true)
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                generateVessels();
            }
            yield return null;
        }
    }

    private IEnumerator UpdateGameObjects(MarineVesselModel[] vessels)
    {
        // Loop over the data and fabricate objects
        foreach (var vessel in vessels)
        {
            // Find a GameObject named "MyObject"
            GameObject sceneVessel = GameObject.Find(vessel.mmsi.ToString());

            if (sceneVessel != null)
            {
                ArcGISLocationComponent vesselLocation = sceneVessel.GetComponent<ArcGISLocationComponent>();
                // Set default location
                vesselLocation.Position = new ArcGISPoint(vessel.geometry.coordinates[0], vessel.geometry.coordinates[1], 90, ArcGISSpatialReference.WGS84());
            }
            else
            {
                // Create a new vehicle object
                GameObject vesselfab = Instantiate(vehiclePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                // Attach ArcGIS location component to the object
                ArcGISLocationComponent vesselLocation = vesselfab.AddComponent<ArcGISLocationComponent>();
                // Set default location
                vesselLocation.Position = new ArcGISPoint(vessel.geometry.coordinates[0], vessel.geometry.coordinates[1], 90, ArcGISSpatialReference.WGS84());
                vesselfab.name = vessel.mmsi.ToString();
                // Add object into the game
                vesselfab.transform.SetParent(gameObject.transform);
                // Enable ArcGIS location component
                vesselLocation.enabled = true;
            }
            yield return null; // Yield to the next frame (main thread)
        }
    }
}
