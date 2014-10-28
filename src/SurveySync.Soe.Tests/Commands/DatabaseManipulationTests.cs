using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using NUnit.Framework;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;
using SurveySync.Soe.Models.Configuration;

namespace SurveySync.Soe.Tests.Commands {

    public class DatabaseManipulationTests {
        [TestFixture]
        public class CreateNewFeatureTests {
            [SetUp]
            public void SetUp()
            {
                ApplicationCache.Settings = new ApplicationSettings
                    {
                        ContributionPropertyPointFields = new ApplicationFields
                            {
                                PropertyId = "PropertyId"
                            }
                    };

                _workspace = TestHelper.GetSdeWorkspace(@"SERVER=localhost\sqlexpress;" +
                                                        "DATABASE=UDSHSpatial_New;" +
                                                        @"INSTANCE=sde:sqlserver:localhost\sqlexpress;" +
                                                        "AUTHENTICATION_MODE=OSA;" +
                                                        "VERSION=dbo.DEFAULT");

                _featureWorkspace = (IFeatureWorkspace) _workspace;
                _sourceTable = _featureWorkspace.OpenFeatureClass("CONTRIBUTION_PROPERTY_POINT");
                _destinationTable = _featureWorkspace.OpenFeatureClass("BuildingWrites");

                _sourceMap = new FeatureClassIndexMap(0, "Contribs", _sourceTable)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"STREET_NAM", new IndexFieldMap(12, "STREET_NAM")}
                            }
                    };

                _destinationMap = new FeatureClassIndexMap(1, "Buildings", _destinationTable)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"STREET_NAM", new IndexFieldMap(12, "STREET_NAM")}
                            }
                    };
            }

            [TearDown]
            public void TearDown()
            {
                var cursor = _destinationTable.Update(_queryFilter, false);
                while (cursor.NextFeature() != null)
                {
                    cursor.DeleteFeature();
                }

                Marshal.ReleaseComObject(cursor);
                Marshal.ReleaseComObject(_sourceTable);
                Marshal.ReleaseComObject(_destinationTable);
                Marshal.ReleaseComObject(_featureWorkspace);
                Marshal.ReleaseComObject(_workspace);
            }

            private IFeatureClass _sourceTable;
            private IFeatureClass _destinationTable;
            private FeatureClassIndexMap _sourceMap;
            private FeatureClassIndexMap _destinationMap;
            private IFeatureWorkspace _featureWorkspace;
            private IWorkspace _workspace;
            private QueryFilter _queryFilter;

            [Test]
            public void CreatesFeatures()
            {
                var propertiesToCreate = new[]
                    {
                        new Schema
                            {
                                ObjectId = 1,
                                PropertyId = 1
                            },
                            new Schema
                            {
                                ObjectId = 2,
                                PropertyId = 10
                            }
                    };

                var command = new CreateNewBuildingRowsCommand(_sourceMap, _destinationMap, propertiesToCreate);
                var actual = CommandExecutor.ExecuteCommand(command);

                _queryFilter = new QueryFilter
                    {
                        WhereClause = "1=1"
                    };

                Assert.That(actual, Is.EqualTo(2));
                Assert.That(_destinationTable.FeatureCount(_queryFilter), Is.EqualTo(2));
            }
        }
    }

}