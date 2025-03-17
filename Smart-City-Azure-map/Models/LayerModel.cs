namespace Smart_City_Azure_map.Models
{
    public class HeatmapLayerModel
    {
        public string type { get; set; }
        public HeatmapLayerFeatureModel[] features { get; set; }
    }

    public class HeatmapLayerFeatureModel
    {
        public string type { get; set; }
        public HeatmapLayerGeometryModel geometry { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public string id { get; set; }
    }

    public class HeatmapLayerGeometryModel
    {
        public string type { get; set; }
        public double[] coordinates { get; set; }
    }

    public class TrafficLayerModel
    {
        public string vendor { get; set; }
        public double[][] path { get; set; }
        public int[] timestamps { get; set; }
    }
}
