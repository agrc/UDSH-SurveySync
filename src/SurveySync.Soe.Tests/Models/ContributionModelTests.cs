using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using Moq;
using NUnit.Framework;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Tests.Models {

    [TestFixture]
    public class ContributionModelTests {
        [SetUp]
        public void SetUp()
        {
            var workspace = TestHelper.GetSdeWorkspace(@"SERVER=localhost\sqlexpress;" +
                                                       "DATABASE=UDSHSpatial_New;" +
                                                       @"INSTANCE=sde:sqlserver:localhost\sqlexpress;" +
                                                       "AUTHENTICATION_MODE=OSA;" +
                                                       "VERSION=dbo.DEFAULT");

            var featureWorkspace = (IFeatureWorkspace) workspace;
            var featureClass = featureWorkspace.OpenFeatureClass("CONTRIBUTION_PROPERTY_POINT");
            var fields = new[]
                {
                    "PROP_NAME", "HOUSE_NO", "DIRECTION", "STREET_NAM", "CITY", "CNTY_CITY", "EST_ADDRES", "EVALUATION",
                    "ORIGINAL_U", "HEIGHT", "TYPE", "OUT_NONCON", "OUT_CONTRI", "POINT_X", "POINT_Y", "COMMENTS"
                };

            _attributeToIndexMap = CommandExecutor.ExecuteCommand(new FindIndexByFieldNameCommand(featureClass, fields));

            var queryFilter = new QueryFilter
                {
                    WhereClause = "OBJECTID = 1"
                };

            var cursor = featureClass.Search(queryFilter, true);

            _feature = cursor.NextFeature();
        }

        private IFeature _feature;
        private IDictionary<string, IndexFieldMap> _attributeToIndexMap;

        [Test]
        public void CanAttributeMap()
        {
            var map = Schema.Map(_feature, _attributeToIndexMap);

            Assert.That(map.BuildingHeight, Is.EqualTo("1"));
            Assert.That(map.BuildingTypeId, Is.EqualTo("RX"));
            Assert.That(map.CityId, Is.EqualTo("SALT LAKE CITY"));
            Assert.That(map.Comments, Is.EqualTo("NEW COLS"));
            Assert.That(map.Contributing, Is.EqualTo("1"));
            Assert.That(map.CountyId, Is.EqualTo("SL21"));
            Assert.That(map.EvaluationCode, Is.EqualTo("B"));
            Assert.That(map.HouseNumber, Is.EqualTo(370));
            Assert.That(map.IsEstimatedAddress, Is.EqualTo("?"));
            Assert.That(map.NonContributing, Is.EqualTo("0"));
            Assert.That(map.OriginalUseId, Is.EqualTo("R1"));
            Assert.That(map.PropertyName, Is.Null);
            Assert.That(map.StreetDirection, Is.EqualTo("N"));
            Assert.That(map.StreetName, Is.EqualTo("MARION STREET"));
            Assert.That(map.XCoord, Is.EqualTo(422286.66000000));
            Assert.That(map.YCoord, Is.EqualTo(4514450.88000000));
        }

        [Test]
        public void CanMapNulls()
        {
            var mock = new Mock<IFeature>();
            mock.Setup(prop => prop.get_Value(8)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(9)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(10)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(11)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(12)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(13)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(14)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(26)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(30)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(31)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(32)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(33)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(34)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(44)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(48)).Returns(DBNull.Value);
            mock.Setup(prop => prop.get_Value(49)).Returns(DBNull.Value);

            var map = Schema.Map(mock.Object, _attributeToIndexMap);

            Assert.That(map.BuildingHeight, Is.Null);
            Assert.That(map.BuildingTypeId, Is.Null);
            Assert.That(map.CityId, Is.Null);
            Assert.That(map.Comments, Is.Null);
            Assert.That(map.Contributing, Is.Null);
            Assert.That(map.CountyId, Is.Null);
            Assert.That(map.EvaluationCode, Is.Null);
            Assert.That(map.HouseNumber, Is.Null);
            Assert.That(map.IsEstimatedAddress, Is.Null);
            Assert.That(map.NonContributing, Is.Null);
            Assert.That(map.OriginalUseId, Is.Null);
            Assert.That(map.PropertyName, Is.Null);
            Assert.That(map.StreetDirection, Is.Null);
            Assert.That(map.StreetName, Is.Null);
            Assert.That(map.XCoord, Is.Null);
            Assert.That(map.YCoord, Is.Null);
        }
    }

}