using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

namespace SurveySync.Soe.Models {

    public class Schema {
        public int ObjectId { get; set; }
        public int PropertyId { get; set; }

        public static Schema Map(IFeature feature, IDictionary<string, IndexFieldMap> attributeToIndexMap)
        {
            var nullNumbers =
                new Func<object, int?>(value => value == DBNull.Value ? (int?) null : Convert.ToInt32(value));

            var propertyId = nullNumbers(feature.Value[attributeToIndexMap["PROPERTYID"].Index]);
            if (!propertyId.HasValue)
            {
                throw new ArgumentException(string.Format("{0}.{1} is missing a property id value.", feature.Table,
                                                          feature.OID));
            }

            var model = new Schema
                {
                    PropertyId = propertyId.Value,
                    ObjectId = feature.OID
                };

            return model;
        }
    }
}