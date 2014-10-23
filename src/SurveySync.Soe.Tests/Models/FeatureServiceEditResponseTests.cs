using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Tests.Models {

    public class FeatureServiceEditResponseTests {
        [TestFixture]
        public class Deserialization {
            [Test]
            public void CanDeserializeEsriSampleJson()
            {
                const string esriSampleJson = "[{\"id\":0,\"addResults\":[{\"objectId\":617,\"success\":true},{\"success\":false,\"error\":{\"code\":-2147217395,\"description\":\"Setting of Value for depth failed.\"}}],\"updateResults\":[{\"objectId\":50,\"success\":true}],\"deleteResults\":[{\"objectId\":19,\"success\":true},{\"objectId\":23,\"success\":true}]},{\"id\":1,\"deleteResults\":[{\"objectId\":34,\"success\":true},{\"objectId\":44,\"success\":true}]}]";

                var actual = JsonConvert.DeserializeObject<IReadOnlyCollection<FeatureServiceEditResponse>>(esriSampleJson);

                Assert.That(actual, Is.Not.Null);

                var idZero = actual.Single(x => x.Id == 0);

                Assert.That(idZero.Id, Is.EqualTo(0));
                Assert.That(idZero.AddResults.Count, Is.EqualTo(2));
                Assert.That(idZero.UpdateResults.Count, Is.EqualTo(1));
                Assert.That(idZero.DeleteResults.Count, Is.EqualTo(2));

                var idOne = actual.Single(x => x.Id == 1);

                Assert.That(idOne.Id, Is.EqualTo(1));
                Assert.That(idOne.AddResults.Count, Is.EqualTo(0));
                Assert.That(idOne.UpdateResults.Count, Is.EqualTo(0));
                Assert.That(idOne.DeleteResults.Count, Is.EqualTo(2));
            }
        }
    }

}