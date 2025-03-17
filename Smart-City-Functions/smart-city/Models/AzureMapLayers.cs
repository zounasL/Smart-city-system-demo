using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart_city.Models
{
    public class AzureMapTrafficPathModel
    {
        public string vendor { get; set; }
        public double[][] path { get; set; }
        public int[] timestamps { get; set; }
    }

    public class AzureMapHeatmapLayer
    {
        public string type { get; set; } = "FeatureCollection";
        public AzureMapHeatmapLayerFeature[] features { get; set; }
    }

    public class AzureMapHeatmapLayerFeature
    {
        public string type { get; set; } = "Feature";
        public AzureMapHeatmapLayerPoint geometry { get; set; }
        public Dictionary<string, string> properties { get; set; } = new Dictionary<string, string>();
        public string id { get; set; }
    }

    public class AzureMapHeatmapLayerPoint
    {
        public string type { get; set; } = "Point";
        public double[] coordinates { get; set; }
    }
}
