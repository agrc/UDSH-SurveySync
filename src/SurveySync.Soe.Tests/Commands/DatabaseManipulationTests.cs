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
            private FeatureClassIndexMap _sourceMap;
            private FeatureClassIndexMap _destinationMap;
            private IFeatureWorkspace _featureWorkspace;
            private IWorkspace _workspace;
            private QueryFilter _queryFilter;

            [Test]
            public void CreatesFeaturesAndReturnsCountCreated()
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

                Assert.That(actual, Is.EqualTo(2));
                Assert.That(_destinationTable.FeatureCount(_queryFilter), Is.EqualTo(2));
            }

            [Test]
            public void CreatesNoFeaturesAndReturnsZeroCountWhenIdIsNotFound()
            {
                var propertiesToCreate = new[]
                    {
                        new Schema
                            {
                                ObjectId = 1,
                                PropertyId = 999
                            }
                    };

                var command = new CreateNewBuildingRowsCommand(_sourceMap, _destinationMap, propertiesToCreate);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.EqualTo(0));
                Assert.That(_destinationTable.FeatureCount(_queryFilter), Is.EqualTo(0));
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

                _sourceMap = new FeatureClassIndexMap(0, "Contribs", _sourceTable)
                    {
                        FieldMap = new Dictionary<string, IndexFieldMap>
                            {
                                {"PropertyId", new IndexFieldMap(50, "PropertyId")},
                                {"ID", new IndexFieldMap(4, "ID")}
                            }
                    };

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
            public void UpdateRowsAndReturnCountModified()
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
                                PropertyId = 2
                            }
                    };

                var command = new UpdateBuildingRowsCommand(_sourceMap, _destinationMap, propertiesToCreate);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.EqualTo(2));

                var cursor = _destinationTable.Search(new QueryFilter
                    {
                        WhereClause = "PropertyID = 1"
                    }, false);

                var actualFeature = cursor.NextFeature();

                Assert.That(actualFeature.Value[_destinationMap.FieldMap["ID"].Index], Is.EqualTo(1518399));

                Marshal.ReleaseComObject(cursor);

                cursor = _destinationTable.Search(new QueryFilter
                {
                    WhereClause = "PropertyID = 2"
                }, false);

                actualFeature = cursor.NextFeature();

                Assert.That(actualFeature.Value[_destinationMap.FieldMap["ID"].Index], Is.EqualTo(1518402));

                Marshal.ReleaseComObject(cursor);
            }
         
            [Test]
            public void UpdateNoRowsAndReturnsZero()
            {
                var propertiesToCreate = new[]
                    {
                        new Schema
                            {
                                ObjectId = 1,
                                PropertyId = 999
                            }
                    };

                var command = new UpdateBuildingRowsCommand(_sourceMap, _destinationMap, propertiesToCreate);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.EqualTo(0));
            }
        }
    }

}