using System.Collections.Generic;
using System.Collections.ObjectModel;
using SurveySync.Soe.Models;
using SurveySync.Soe.Models.Configuration;
using ESRI.ArcGIS.Geodatabase;

namespace SurveySync.Soe.Cache {
    /// <summary>
    ///     The global cache for the application
    /// </summary>
    public static class ApplicationCache {
        /// <summary>
        ///     The property value index map
        /// </summary>
        public static Dictionary<IFeatureClass, IndexFieldMap> PropertyValueIndexMap;

        /// <summary>
        ///     Gets or sets the feature class index map.
        /// </summary>
        /// <value>
        ///     The feature class index map.
        /// </value>
        public static Collection<FeatureClassIndexMap> FeatureClassIndexMap { get; set; }

        /// <summary>
        ///     Gets or sets the fields used in the application.
        /// </summary>
        /// <value>
        ///     The fields.
        /// </value>
        public static ApplicationSettings Settings { get; set; }
    }
}
