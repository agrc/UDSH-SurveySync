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

        /// <summary>
        /// Gets or sets the name of the survey to property identifier lookup table.
        /// </summary>
        /// <value>
        /// The survey to property identifier lookup table.
        /// </value>
        public string SurveyToPropertyIdLookupTable { get; set; }

        /// <summary>
        /// Gets or sets the survey to property identifier fields used to query and link records.
        /// </summary>
        /// <value>
        /// The survey to property identifier fields.
        /// </value>
        public ApplicationFields SurveyToPropertyIdFields { get; set; }

        /// <summary>
        /// Gets or sets the contribution property point fields used to query and link records.
        /// </summary>
        /// <value>
        /// The contribution property point fields.
        /// </value>
        public ApplicationFields ContributionPropertyPointFields { get; set; }

        /// <summary>
        /// Gets or sets the name of the contribution property point layer in the mxd.
        /// </summary>
        /// <value>
        /// The name of the contribution property point layer.
        /// </value>
        public string ContributionPropertyPointLayerName { get; set; }

        /// <summary>
        /// Gets or sets the feature service editing URL that contains the two editing layers.
        /// </summary>
        /// <value>
        /// The feature service editing URL.
        /// </value>
        public string FeatureServiceEditingUrl { get; set; }

        /// <summary>
        /// Gets or sets the building fields used to query and link data.
        /// </summary>
        /// <value>
        /// The building fields.
        /// </value>
        public ApplicationFields BuildingFields { get; set; }

        /// <summary>
        /// Gets or sets the name of the building layer in the mxd.
        /// </summary>
        /// <value>
        /// The name of the building layer.
        /// </value>
        public string BuildingLayerName { get; set; }

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