using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.View;
using System.Collections.Generic;
using UnityEngine;

public class VehicleRouteHandler : MonoBehaviour
{
    private float nextActionTime = 0.0f;
    private int step = 0;
    public float period = 0.1f;
    public bool initialized = false;
    public List<double[]> path = new List<double[]>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(initialized) { 
            // Wait wanted time before rerendering object location
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                ArcGISMapComponent arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
                ArcGISView view = arcGISMapComponent.View;
                // Chek that ArcGIS map has SpatialReference and object has more path locations
                if (view.SpatialReference != null && step < path.Count)
                {
                    // Parse next location
                    double[] coordinates = path[step];
                    // Create next point and asume that elevation is not enabled
                    ArcGISPoint point = new ArcGISPoint(coordinates[0], coordinates[1], 90f, ArcGISSpatialReference.WGS84());
                    // Get game object location component
                    ArcGISLocationComponent locationComponent = gameObject.GetComponent<ArcGISLocationComponent>();
                    // Set new location
                    locationComponent.Position = point;
                    // increase step count
                    step += 1;
                }
            }
        }
    }
}
