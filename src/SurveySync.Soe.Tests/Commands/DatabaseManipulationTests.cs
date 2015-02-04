using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using CommandPattern;
using ESRI.ArcGIS.Geodatabase;
using NUnit.Framework;
using SurveySync.Soe.Cache;
using SurveySync.Soe.Commands.Http;
using SurveySync.Soe.Models;
using SurveySync.Soe.Models.Configuration;
using SurveySync.Soe.Models.FeatureService;

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

                new FeatureClassIndexMap(0, "Contribs", _sourceTable)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"STREET_NAM", new IndexFieldMap(12, "STREET_NAM")}
                            }
                    };

                new FeatureClassIndexMap(1, "Buildings", _destinationTable)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"STREET_NAM", new IndexFieldMap(12, "STREET_NAM")}
                            }
                    };

                const string getAllWhereClause = "1=1";
                _queryFilter = new QueryFilter
                    {
                        WhereClause = getAllWhereClause
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
            private IFeatureWorkspace _featureWorkspace;
            private IWorkspace _workspace;
            private QueryFilter _queryFilter;
            private const string Url = "http://localhost/arcgis/rest/services/UDSH/SurveySync/FeatureServer/applyEdits";
            private const int BuildingWritesIndex = 3;

            [Test]
            public void CreatesFeaturesAndReturnsCountCreated()
            {
                var container = new EditContainer(new[]
                    {
                        new ApplyEditActions
                            {
                                Id = BuildingWritesIndex,
                                Adds = new Collection<FeatureAction>
                                    {
                                        new FeatureAction(new Dictionary<string, object>
                                            {
                                                {"PropertyId", 1}
                                            }),
                                        new FeatureAction(new Dictionary<string, object>
                                            {
                                                {"PropertyId", 10}
                                            })
                                    }
                            }
                    });

                var command = new SendEditsToFeatureServiceCommand(container, Url);
                var response = CommandExecutor.ExecuteCommand(command);

                var actual = new SoeResponse(response);

                Assert.That(actual.Created, Is.EqualTo(2));
                Assert.That(actual.Updated, Is.EqualTo(0));
                Assert.That(actual.Deleted, Is.EqualTo(0)); 
                Assert.That(actual.Successful, Is.True);

                Assert.That(_destinationTable.FeatureCount(_queryFilter), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class UpdateBuildingTests {
            [SetUp]
            public void SeUp()
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
                _destinationTable = _featureWorkspace.OpenFeatureClass("BuildingsUpdates");

                _destinationMap = new FeatureClassIndexMap(1, "Buildings", _destinationTable)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"PropertyId", new IndexFieldMap(50, "PropertyId")},
                                {"ID", new IndexFieldMap(4, "ID")}
                            }
                    };

                const string getAllWhereClause = "1=1";
                _queryFilter = new QueryFilter
                    {
                        WhereClause = getAllWhereClause
                    };

                const bool useBuffering = false;

                var insertCursor = _destinationTable.Insert(useBuffering);
                var featureBuffer = _destinationTable.CreateFeatureBuffer();

                featureBuffer.Value[_destinationMap.FieldMap["PropertyId"].Index] = 1;
                insertCursor.InsertFeature(featureBuffer);
                featureBuffer.Value[_destinationMap.FieldMap["PropertyId"].Index] = 2;
                insertCursor.InsertFeature(featureBuffer);

                insertCursor.Flush();

                Marshal.ReleaseComObject(featureBuffer);
                Marshal.ReleaseComObject(insertCursor);

                _objectIds = new List<int>();

                var cursor = _destinationTable.Search(_queryFilter, false);
                IFeature feature;
                while ((feature = cursor.NextFeature()) != null)
                {
                    _objectIds.Add(feature.OID);
                }
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
            private FeatureClassIndexMap _destinationMap;
            private IFeatureWorkspace _featureWorkspace;
            private IWorkspace _workspace;
            private QueryFilter _queryFilter;
            private const string Url = "http://localhost/arcgis/rest/services/UDSH/SurveySync/FeatureServer/applyEdits";
            private const int BuildingUpdatesIndex = 2;
            private List<int> _objectIds;

            /// <summary>
            ///     Updates the no rows and returns zero.
            /// </summary>
            [Test]
            public void UpdateNoRowsAndReturnsZero()
            {
                var container = new EditContainer(new[]
                    {
                        new ApplyEditActions
                            {
                                Id = BuildingUpdatesIndex,
                                Updates = new Collection<FeatureAction>
                                    {
                                        new FeatureAction(new Dictionary<string, object>
                                            {
                                                {"PropertyId", 999},
                                                {"OBJECTID", 999}
                                            })
                                    }
                            }
                    });

                var command = new SendEditsToFeatureServiceCommand(container, Url);
                var response = CommandExecutor.ExecuteCommand(command);

                var actual = new SoeResponse(response);

                Assert.That(actual.Successful, Is.False);
                Assert.That(actual.Message, Is.EqualTo("Unable to complete operation."));
                Assert.That(actual.Updated, Is.EqualTo(0));
                Assert.That(actual.Created, Is.EqualTo(0));
                Assert.That(actual.Deleted, Is.EqualTo(0));
            }

            [Test]
            public void UpdateRowsAndReturnCountModified()
            {
                var container = new EditContainer(new[]
                    {
                        new ApplyEditActions
                            {
                                Id = BuildingUpdatesIndex,
                                Updates = new Collection<FeatureAction>
                                    {
                                        new FeatureAction(new Dictionary<string, object>
                                            {
                                                {"ID", 1},
                                                {"OBJECTID", _objectIds.First()}
                                            }),
                                        new FeatureAction(new Dictionary<string, object>
                                            {
                                                {"ID", 2},
                                                {"OBJECTID", _objectIds.Last()}
                                            })
                                    }
                            }
                    });

                var command = new SendEditsToFeatureServiceCommand(container, Url);
                var response = CommandExecutor.ExecuteCommand(command);

                var actual = new SoeResponse(response);

                Assert.That(actual.Successful, Is.True);
                Assert.That(actual.Message, Is.Empty);
                Assert.That(actual.Updated, Is.EqualTo(2));
                Assert.That(actual.Created, Is.EqualTo(0));
                Assert.That(actual.Deleted, Is.EqualTo(0));

                var cursor = _destinationTable.Search(new QueryFilter
                    {
                        WhereClause = "PropertyID = 1"
                    }, false);

                var actualFeature = cursor.NextFeature();

                Assert.That(actualFeature.Value[_destinationMap.FieldMap["ID"].Index], Is.EqualTo(1));

                Marshal.ReleaseComObject(cursor);

                cursor = _destinationTable.Search(new QueryFilter
                    {
                        WhereClause = "PropertyID = 2"
                    }, false);

                actualFeature = cursor.NextFeature();

                Assert.That(actualFeature.Value[_destinationMap.FieldMap["ID"].Index], Is.EqualTo(2));

                Marshal.ReleaseComObject(cursor);
            }
        }
    }

}