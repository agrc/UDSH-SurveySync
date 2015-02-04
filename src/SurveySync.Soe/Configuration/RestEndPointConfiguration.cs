using ESRI.ArcGIS.esriSystem;
using SurveySync.Soe.Extensions;
using SurveySync.Soe.Infastructure;
using SurveySync.Soe.Models.Configuration;

namespace SurveySync.Soe.Configuration {

    public class RestEndPointConfiguration : IConfigurable {
        public ApplicationSettings GetSettings(IPropertySet props)
        {
            var settings = new ApplicationSettings
                {
                    FeatureServiceEditingUrl = props.GetValueAsString("featureServiceUrl"),
                    SurveyToPropertyIdLookupTable = props.GetValueAsString("propertySurveyRecordTableName"),
                    SurveyToPropertyIdFields = new ApplicationFields
                        {
                            PropertyId = props.GetValueAsString("survey.PropertyId"),
                            SurveyId = props.GetValueAsString("survey.SurveyId"),
                            ReturnFields = props.GetValueAsString("survey.ReturnFields").Split(new[] { ',' }),
                        },
                    ContributionPropertyPointFields = new ApplicationFields
                        {
                            PropertyId = props.GetValueAsString("contributionPropertyPoint.PropertyId"),
                        },
                    ContributionPropertyPointLayerName = props.GetValueAsString("contributionPropertyPoint.LayerName"),
                    BuildingFields = new ApplicationFields
                        {
                            PropertyId = props.GetValueAsString("buildings.PropertyId"),
                        },
                    BuildingLayerName = props.GetValueAsString("buildings.LayerName"),
                    ConnectionString = props.GetValueAsString("connectionString").Replace("::", ";").Replace("--", "=").Replace("\\\\", "\\"),
                };

            return settings;
        }
    }

}