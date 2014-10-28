﻿using System.Collections.Generic;
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
                                {"PROPERTYID", new IndexFieldMap(0, "PROPERTYID")},
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