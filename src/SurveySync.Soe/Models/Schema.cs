using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

namespace SurveySync.Soe.Models {

    public class Schema {
        public string BuildingHeight { get; set; }
        public string BuildingTypeId { get; set; }
        public string CityId { get; set; }
        public string Comments { get; set; }
        public string Contributing { get; set; }
        public string CountyId { get; set; }
        public string EvaluationCode { get; set; }
        public double? HouseNumber { get; set; }
        public string IsEstimatedAddress { get; set; }
        public string NonContributing { get; set; }
        public string OriginalUseId { get; set; }
        public string PropertyName { get; set; }
        public string StreetDirection { get; set; }
        public string StreetName { get; set; }
        public int ObjectId { get; set; }
        public double? XCoord { get; set; }
        public double? YCoord { get; set; }

        public static Schema Map(IFeature feature, IDictionary<string, IndexFieldMap> attributeToIndexMap)
        {
            var nullStrings = new Func<object, string>(value => value == DBNull.Value ? null : value.ToString());
            var nullNumbers = new Func<object, double?>(value => value == DBNull.Value ? (double?) null : Convert.ToDouble(value));

            var model = new Schema
                {
                    BuildingHeight = nullStrings(feature.Value[attributeToIndexMap["HEIGHT"].Index]),
                    BuildingTypeId = nullStrings(feature.Value[attributeToIndexMap["TYPE"].Index]),
                    CityId = nullStrings(feature.Value[attributeToIndexMap["CITY"].Index]),
                    Comments = nullStrings(feature.Value[attributeToIndexMap["COMMENTS"].Index]),
                    Contributing = nullStrings(feature.Value[attributeToIndexMap["OUT_CONTRI"].Index]),
                    CountyId = nullStrings(feature.Value[attributeToIndexMap["CNTY_CITY"].Index]),
                    EvaluationCode = nullStrings(feature.Value[attributeToIndexMap["EVALUATION"].Index]),
                    HouseNumber = nullNumbers(feature.Value[attributeToIndexMap["HOUSE_NO"].Index]),
                    IsEstimatedAddress = nullStrings(feature.Value[attributeToIndexMap["EST_ADDRES"].Index]),
                    NonContributing = nullStrings(feature.Value[attributeToIndexMap["OUT_NONCON"].Index]),
                    OriginalUseId = nullStrings(feature.Value[attributeToIndexMap["ORIGINAL_U"].Index]),
                    PropertyName = nullStrings(feature.Value[attributeToIndexMap["PROP_NAME"].Index]),
                    StreetDirection = nullStrings(feature.Value[attributeToIndexMap["DIRECTION"].Index]),
                    StreetName = nullStrings(feature.Value[attributeToIndexMap["STREET_NAM"].Index]),
                    XCoord = nullNumbers(feature.Value[attributeToIndexMap["POINT_X"].Index]),
                    YCoord = nullNumbers(feature.Value[attributeToIndexMap["POINT_Y"].Index]),
                };

            return model;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return
                string.Format(
                    "(PropertyName, PROP_NAME): {0},{16}" +
                    "(HouseNumber, HOUSE_NO): {1},{16}" +
                    "(StreetDirection, DIRECTION): {2},{16}" +
                    "(StreetName, STREET_NAM): {3},{16}" +
                    "(CityId, CITY): {4},{16}" +
                    "(CountyId, CNTY_CITY): {5},{16}" +
                    "(IsEstimatedAddress, EST_ADDRES): {6},{16}" +
                    "(EvaluationCode, EVALUATION): {7},{16}" +
                    "(OriginalUseId, ORIGINAL_U): {8},{16}" +
                    "(BuildingHeight, HEIGHT): {9},{16}" +
                    "(BuildingTypeId, TYPE): {10},{16}" +
                    "(NonContributing, OUT_NONCON): {11},{16}" +
                    "(Contributing, OUT_CONTRI): {12},{16}" +
                    "(XCoord, POINT_X): {13},{16}" +
                    "(YCoord, POINT_Y): {14},{16}" +
                    "(Comments, COMMENTS): {15}",
                    PropertyName, HouseNumber, StreetDirection, StreetName, CityId, CountyId, IsEstimatedAddress,
                    EvaluationCode, OriginalUseId, BuildingHeight, BuildingTypeId, NonContributing, Contributing, XCoord,
                    YCoord, Comments, Environment.NewLine);
        }
    }

}