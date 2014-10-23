using System;

namespace SurveySync.Soe.Models.Configuration {

    /// <summary>
    ///     The class representing Application settings
    ///     This can be modified to suite the application needs or removed.
    /// </summary>
    public class ApplicationSettings {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        /// <value>
        ///     The connection string for the tabular UDSH data.
        /// </value>
        public string ConnectionString { get; set; }

        public string SurveyToPropertyIdLookupTable { get; set; }

        public ApplicationFields SurveyToPropertyIdFields { get; set; }

        public ApplicationFields ContributionPropertyPointFields { get; set; }

        public string FeatureServiceEditingUrl { get; set; }

        public ApplicationFields BuildingFields { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("ConnectionString: {0},{3} SurveyToPropertyIdLookupTable: {1},{3} SurveyToPropertyIdFields: {2}", ConnectionString, SurveyToPropertyIdLookupTable, SurveyToPropertyIdFields, Environment.NewLine);
        }
    }

}