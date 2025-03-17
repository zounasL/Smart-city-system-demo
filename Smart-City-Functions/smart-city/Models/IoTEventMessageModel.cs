using Microsoft.Azure.Amqp;
using Microsoft.Azure.Cosmos.Spatial;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;

namespace smart_city.Models
{
    public class IoTEventMessageModel
    {
        public string deviceId { get; set; }
        public string data { get; set; }
        public string coordinates { get; set; }
        public double[] getCoordinates()
        {
            return coordinates.Split(',')
            .Select(value => {
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return result;
                }
                return double.NaN;
            })
            .ToArray();
        }
        public Point getPoint() {
            double[] latLong = getCoordinates();
            Point point = new Point(latLong[0], latLong[1]);
            return point;
        }

        public IoTEventCosmosModel getCosmosDocumentModel ()
        {
            return  new IoTEventCosmosModel
            {
                id = Guid.NewGuid().ToString(),
                deviceId = deviceId,
                location = getPoint(),
                timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                data = JsonConvert.DeserializeObject<IoTEventCosmosModelData>(data)
            };
        }
    }

    public class IoTEventCosmosModel
    {
        public string id { get; set; }
        public string deviceId { get; set; }
        public string timestamp { get; set; }
        public Point location { get; set; }
        public IoTEventCosmosModelData data { get; set; }
    }

    public class IoTEventCosmosModelData
    {
        public int vehicles { get; set; }
    }
}
