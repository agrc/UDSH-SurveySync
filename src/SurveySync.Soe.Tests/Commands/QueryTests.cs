using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using NUnit.Framework;
using SurveySync.Soe.Commands.Searches;
using SurveySync.Soe.Configuration;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Tests.Commands {

    public class QueryTests {
        [TestFixture]
        public class GetContributionsCommandTests {
            [SetUp]
            public void SetUp()
            {
                var workspace = TestHelper.GetSdeWorkspace(@"SERVER=localhost\sqlexpress;" +
                                                           "DATABASE=UDSHSpatial_New;" +
                                                           @"INSTANCE=sde:sqlserver:localhost\sqlexpress;" +
                                                           "AUTHENTICATION_MODE=OSA;" +
                                                           "VERSION=dbo.DEFAULT");

                var featureWorkspace = (IFeatureWorkspace) workspace;
                _featureClass = featureWorkspace.OpenFeatureClass("CONTRIBUTION_PROPERTY_POINT");
            }

            private IFeatureClass _featureClass;

            [Test]
            public void GetsContributionFromQuery()
            {
                var map = new FeatureClassIndexMap(0, "cpp", _featureClass)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"PROP_NAME", new IndexFieldMap(0, "PROP_NAME")},
                                {"HOUSE_NO", new IndexFieldMap(0, "HOUSE_NO")},
                                {"DIRECTION", new IndexFieldMap(0, "DIRECTION")},
                                {"STREET_NAM", new IndexFieldMap(0, "STREET_NAM")},
                                {"CITY", new IndexFieldMap(0, "CITY")},
                                {"CNTY_CITY", new IndexFieldMap(0, "CNTY_CITY")},
                                {"EST_ADDRES", new IndexFieldMap(0, "EST_ADDRES")},
                                {"EVALUATION", new IndexFieldMap(0, "EVALUATION")},
                                {"ORIGINAL_U", new IndexFieldMap(0, "ORIGINAL_U")},
                                {"HEIGHT", new IndexFieldMap(0, "HEIGHT")},
                                {"TYPE", new IndexFieldMap(0, "TYPE")},
                                {"OUT_NONCON", new IndexFieldMap(0, "OUT_NONCON")},
                                {"OUT_CONTRI", new IndexFieldMap(0, "OUT_CONTRI")},
                                {"POINT_X", new IndexFieldMap(0, "POINT_X")},
                                {"POINT_Y", new IndexFieldMap(0, "POINT_Y")},
                                {"COMMENTS", new IndexFieldMap(0, "COMMENTS")},
                            }
                    };

                const string whereClause = "PropertyId in (1,2,3)";
                var command = new GetRecordsCommand(map, whereClause);

                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Count, Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class GetPropertyIdsFromSurveyCommandTests {
            [Test]
            public void GetsIdsFromValidQuery()
            {
                const int validSurvey = 126;

                var settings = new DebugConfiguration().Settings;
                var command = new GetPropertyIdsFromSurveyCommand(settings, validSurvey);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Length, Is.EqualTo(10));
            }

            [Test]
            public void ReturnsNullIfNoPropertyIdsFound()
            {
                const int invalidSurvey = -1;

                var settings = new DebugConfiguration().Settings;
                var command = new GetPropertyIdsFromSurveyCommand(settings, invalidSurvey);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Null);
            }
        }
    }

}