using Newtonsoft.Json;

namespace SurveySync.Soe.Models.FeatureService {

    public class Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }
    }

}