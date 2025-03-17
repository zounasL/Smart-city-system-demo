using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace smart_city.Models
{
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
        public long timestampExternal { get; set; }

        public bool IsTimestampOlderThanDays(int day)
        {
            DateTime timestampDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestampExternal);
            TimeSpan timeDifference = DateTime.Now - timestampDate;
            return timeDifference.TotalDays > day;
        }
    }

    public class AISModel 
    { 
        public MarineVesselModel[] features { get; set; }
    }
}
