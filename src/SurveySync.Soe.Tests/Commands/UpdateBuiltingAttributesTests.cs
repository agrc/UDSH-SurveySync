using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Tests.Commands
{
    [TestFixture]
    public class UpdateBuiltingAttributesTests
    {
        [Test]
        public void UpdatesAttributes()
        {
            var source = new[]
            {
                new FeatureAction(new Dictionary<string, object>
                {
                    {"PropertyId", 1},
                    {"OBJECTID", "don't be this value"},
                    {"Custom", "source"}
                })
            };

            var destination = new[]
            {
                new FeatureAction(new Dictionary<string, object>
                {
                    {"PropertyId", 1},
                    {"OBJECTID", "don't change"},
                    {"Custom", "destination"}
                })
            };

            var command= new UpdateBuildingAttributesCommand(source, destination);

            command.Execute();

            var result = command.Result;

            Assert.That(result.First().Attributes["Custom"], Is.EqualTo("source"));
            Assert.That(result.First().Attributes["OBJECTID"], Is.EqualTo("don't change"));
        }
    }
}