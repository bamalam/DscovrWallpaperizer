using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace DailyDscovrConsoleApp
{
    [DataContract]
    internal class ImageData
    {
        [DataMember]
        public string image { get; set; }
        [DataMember]
        public string caption { get; set; }
        [DataMember]
        public string coords { get; set; }
        [DataMember]
        public DateTime date { get; set; }

        [DataMember]
        public Coordinate centroid_coordinates { get; set; }

        [DataMember]
        public xyzPosition dscovr_j2000_position { get; set; }

        [DataMember]
        public xyzPosition lunar_j2000_position { get; set; }

        [DataMember]
        public xyzPosition sun_j2000_position { get; set; }

        [DataMember]
        public Quaternions attitude_quaternions { get; set; }

        private ImageCoordinates _coordinates;
        public ImageCoordinates coordinates
        {
            get
            {
                // TODO this has since been broken...
                // this is because the json is escaped... o_0
                if (_coordinates != null) return _coordinates;
                JToken token = JToken.Parse(coords);
                _coordinates = token.ToObject<ImageCoordinates>();

                return _coordinates;
            }
        }

    }
}
