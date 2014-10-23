using ESRI.ArcGIS.esriSystem;
using SurveySync.Soe.Extensions;
using SurveySync.Soe.Infastructure;
using SurveySync.Soe.Models.Configuration;

namespace SurveySync.Soe.Configuration {

    #region License

    // 
    // Copyright (C) 2012 AGRC
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
    // and associated documentation files (the "Software"), to deal in the Software without restriction, 
    // including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    // and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
    // subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all copies or substantial 
    // portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
    // NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    // IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
    // SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    // 

    #endregion

    /// <summary>
    ///     Debug configration. Preconfigured for debug environment
    /// </summary>
    public class DebugConfiguration : IConfigurable {
        public ApplicationSettings GetSettings(IPropertySet set)
        {
            var settings = new ApplicationSettings
                {
                    SurveyToPropertyIdLookupTable = "PROPERTYSURVEYRECORD",
                    FeatureServiceEditingUrl = "http://localhost/arcgis/rest/services/UDSH/soe/FeatureServer/applyEdits",
                    SurveyToPropertyIdFields = new ApplicationFields
                        {
                            PropertyId = "PropertyRecordID",
                            SurveyId = "SurveyRecordID",
                            ReturnFields = new[] {"PropertyRecordID"}
                        },
                    ContributionPropertyPointFields = new ApplicationFields
                    {
                        PropertyId = "PropertyId"
                    },
                    ConnectionString =
                        @"Data Source=localhost\sqlexpress;Initial Catalog=UDSHHistoricBuildings;Trusted_Connection=Yes;"
                };

            return settings;
        }
    }

    public class RestEndPointConfiguration : IConfigurable {
        public ApplicationSettings GetSettings(IPropertySet props)
        {
            var settings = new ApplicationSettings
                {
                    FeatureServiceEditingUrl = props.GetValueAsString("featureServiceUrl", true),
                    SurveyToPropertyIdLookupTable = props.GetValueAsString("programid", true),
                    SurveyToPropertyIdFields = new ApplicationFields
                        {
                            PropertyId = "PropertyRecordID",
                            SurveyId = "SurveyRecordID",
                            ReturnFields = new[] {"PropertyRecordID"}
                        },
                    ContributionPropertyPointFields = new ApplicationFields
                    {
                        PropertyId = "PropertyId"
                    },
                    ConnectionString =
                        @"Data Source=localhost\sqlexpress;Initial Catalog=UDSHHistoricBuildings;Trusted_Connection=Yes;",
                };

            return settings;
        }
    }

}