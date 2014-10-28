using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Infastructure.Commands;
using SurveySync.Soe.Models;

namespace SurveySync.Soe.Tests.Commands {

    [TestFixture]
    public class CursorTaskTests {
        [Test]
        public void ActionIsNullIfContributionsAreEmpty()
        {
            var contributions = new List<Schema>();
            var buildings = new List<Schema>
                {
                    new Schema {ObjectId = 1, PropertyId = 3},
                    new Schema {ObjectId = 2, PropertyId = 4},
                    new Schema {ObjectId = 3, PropertyId = 5},
                };

            var command = new CreateCursorTasksCommand(contributions, buildings);
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void ActionIsNullIfListsAreEmpty()
        {
            var command = new CreateCursorTasksCommand(new List<Schema>(), new List<Schema>());
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void AddsCreateActionIfIdsDoNotExist()
        {
            var contributions = new List<Schema>
                {
                    new Schema {ObjectId = 0, PropertyId = 1},
                    new Schema {ObjectId = 0, PropertyId = 2}
                };
            var buildings = new List<Schema>
                {
                    new Schema {ObjectId = 1, PropertyId = 3},
                    new Schema {ObjectId = 2, PropertyId = 4},
                    new Schema {ObjectId = 3, PropertyId = 5},
                };

            var command = new CreateCursorTasksCommand(contributions, buildings);
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual.Update, Is.Empty);
            Assert.That(actual.Create, Is.Not.Empty);
            Assert.That(actual.Create.Count(), Is.EqualTo(2));
        }

        [Test]
        public void AddsUpdateActionIfIdsExistInBothSets()
        {
            var contributions = new List<Schema>
                {
                    new Schema {ObjectId = 0, PropertyId = 1},
                    new Schema {ObjectId = 1, PropertyId = 2}
                };
            var buildings = new List<Schema>
                {
                    new Schema {ObjectId = 1, PropertyId = 1},
                    new Schema {ObjectId = 2, PropertyId = 2},
                    new Schema {ObjectId = 3, PropertyId = 3},
                };

            var command = new CreateCursorTasksCommand(contributions, buildings);
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual.Create, Is.Empty);
            Assert.That(actual.Update, Is.Not.Empty);
            Assert.That(actual.Update.Count(), Is.EqualTo(2));
        }
    }

}