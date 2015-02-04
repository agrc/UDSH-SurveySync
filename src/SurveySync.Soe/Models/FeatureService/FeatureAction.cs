using System.Collections;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Newtonsoft.Json;

namespace SurveySync.Soe.Models.FeatureService {

    public class FeatureAction {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureAction"/> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <param name="geometry">The geometry.</param>
        public FeatureAction(Dictionary<string, object> attributes, Point geometry=null)
        {
            Geometry = geometry;
            Attributes = attributes;
        }

        /// <summary>
        ///     Gets or sets the point geometry.
        /// </summary>
        /// <value>
        ///     The point geometry.
        /// </value>
        [JsonProperty(PropertyName = "geometry")]
        public Point Geometry { get; set; }

        /// <summary>
        ///     Gets or sets the key value pair attributes for the feature class.
        /// </summary>
        /// <value>
        ///     The attributes.
        /// </value>
        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        public static FeatureAction Create(IFeature feature, IList<string> ignores = null)
        {
            var attributes = new Dictionary<string, object>();
            var fields = feature.Fields;

            for (var i = 0; i < fields.FieldCount; i++)
            {
                var field = fields.Field[i];

                if (!field.Editable && field.Name != "OBJECTID") 
                {
                    continue;
                }

                attributes.Add(field.Name, feature.Value[i]);
            }

            var point = feature.ShapeCopy as IPoint;

            if (point == null)
            {
                return new FeatureAction(attributes);
            }

            return new FeatureAction(attributes, new Point(point.X, point.Y));
        }
    }

}