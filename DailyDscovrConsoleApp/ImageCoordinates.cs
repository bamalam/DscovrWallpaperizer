using System.Runtime.Serialization;

namespace DailyDscovrConsoleApp
{
    [DataContract]
    public class ImageCoordinates
    {
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
    }


    [DataContract]
    public class Coordinate
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lon { get; set; }
    }


    [DataContract]
    public class xyzPosition
    {
        [DataMember]
        public double x { get; set; }
        [DataMember]
        public double y { get; set; }
        [DataMember]
        public double z { get; set; }
    }

    [DataContract]
    public class Quaternions
    {
        [DataMember]
        public double q0 { get; set; }
        [DataMember]
        public double q1 { get; set; }
        [DataMember]
        public double q2 { get; set; }
        [DataMember]
        public double q3 { get; set; }
    }
}
