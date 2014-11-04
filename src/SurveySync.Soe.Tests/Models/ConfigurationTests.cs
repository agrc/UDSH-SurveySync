using ESRI.ArcGIS.esriSystem;
using Moq;
using NUnit.Framework;
using SurveySync.Soe.Configuration;

namespace SurveySync.Soe.Tests.Models {

    [TestFixture]
    public class ConfigurationTests {
        private Mock<IPropertySet> _propertySet;
        private RestEndPointConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            _propertySet = new Mock<IPropertySet>();
            _config = new RestEndPointConfiguration();
        }

        [Test]
        public void CanParseConnectionString()
        {
            _propertySet.Setup(x => x.GetProperty("ConnectionString")).Returns(
                "Data Source--localhost\\\\sqlexpress::Initial Catalog--UDSHHistoricBuildings::Trusted_Connection--Yes::");

            var propertySet = _propertySet.Object;

            var actual = _config.GetSettings(propertySet);

            Assert.That(actual.ConnectionString, Is.EqualTo("Data Source=localhost\\sqlexpress;Initial Catalog=UDSHHistoricBuildings;Trusted_Connection=Yes;"));
        }

        [Test]
        public void CanParseMultipleReturnFields()
        {
            _propertySet.Setup(x => x.GetProperty("Survey.ReturnFields")).Returns("A,B");

            var propertySet = _propertySet.Object;

            var actual = _config.GetSettings(propertySet);

            Assert.That(actual.SurveyToPropertyIdFields.ReturnFields.Length, Is.EqualTo(2));
            Assert.That(actual.SurveyToPropertyIdFields.ReturnFields, Is.EquivalentTo(new[]{"A","B"}));
        }

        [Test]
        public void CanParseSongleReturnFields()
        {
            _propertySet.Setup(x => x.GetProperty("Survey.ReturnFields")).Returns("A");

            var propertySet = _propertySet.Object;

            var actual = _config.GetSettings(propertySet);

            Assert.That(actual.SurveyToPropertyIdFields.ReturnFields.Length, Is.EqualTo(1));
            Assert.That(actual.SurveyToPropertyIdFields.ReturnFields, Is.EquivalentTo(new[]{"A"}));
        }
    }

}