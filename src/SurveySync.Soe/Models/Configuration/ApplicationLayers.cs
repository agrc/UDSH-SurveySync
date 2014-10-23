using ESRI.ArcGIS.Geodatabase;

namespace SurveySync.Soe.Models.Configuration {

    public class ApplicationLayers {
        public IFeatureClass ContributionPropertyPoint { get; set; }
        public IFeatureClass Buildings { get; set; }
    }

}