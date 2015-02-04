using System;
using System.Collections.Generic;
using System.Linq;
using CommandPattern;
using NUnit.Framework;
using SurveySync.Soe.Commands;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Tests.Commands {

    [TestFixture]
    public class ApplyEditActionTests {
        [Test]
        public void ActionIsNullIfContributionsAreEmpty()
        {
            var contributions = new List<FeatureAction>();
            var buildings = new List<FeatureAction>
                {
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"PropertyId", 3}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"PropertyId", 4}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"PropertyId", 5}
                        }),
                };

            var command = new CreateApplyEditActionsCommand(contributions, 0, buildings, 1);
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void ActionIsNullIfListsAreEmpty()
        {
            var command = new CreateApplyEditActionsCommand(new List<FeatureAction>(), 0, new List<FeatureAction>(), 0);
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void AddsCreateActionIfIdsDoNotExist()
        {
            var contributions = new List<FeatureAction>
                {
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 1},
                            {"PropertyId", 1}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 2},
                            {"PropertyId", 2}
                        })
                };
            var buildings = new List<FeatureAction>
                {
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 1},
                            {"PropertyId", 3}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 2},
                            {"PropertyId", 4}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 3},
                            {"PropertyId", 5}
                        }),
                };

            var command = new CreateApplyEditActionsCommand(contributions, 0, buildings, 1);
            var actual = CommandExecutor.ExecuteCommand(command);

            var applyEditActions = actual.Edits.Single(x => x.Id == 1);
            var deleteEditActions = actual.Edits.Single(x => x.Id == 0);

            Assert.That(applyEditActions.Updates, Is.Empty);
            Assert.That(applyEditActions.Adds, Is.Not.Empty);
            Assert.That(applyEditActions.Adds.Count, Is.EqualTo(2));
            Assert.That(deleteEditActions.Deletes.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddsUpdateActionIfIdsExistInBothSets()
        {
            var contributions = new List<FeatureAction>
                {
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 1},
                            {"PropertyId", 1},
                            {"UpdateMe", "To This"}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 2},
                            {"PropertyId", 2},
                            {"UpdateMe", "With This"}
                        })
                };
            var buildings = new List<FeatureAction>
                {
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 1},
                            {"PropertyId", 1},
                            {"UpdateMe", "From This"}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 2},
                            {"PropertyId", 2},
                            {"UpdateMe", ""}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"OBJECTID", 3},
                            {"PropertyId", 3}
                        })
                };

            var command = new CreateApplyEditActionsCommand(contributions, 0, buildings, 1);
            var actual = CommandExecutor.ExecuteCommand(command);

            var applyEditActions = actual.Edits.Single(x => x.Id == 1);
            var deleteEditActions = actual.Edits.Single(x => x.Id == 0);

            Assert.That(applyEditActions.Adds, Is.Empty);
            Assert.That(applyEditActions.Updates, Is.Not.Empty);
            Assert.That(applyEditActions.Updates.Count, Is.EqualTo(2));
            Assert.That(deleteEditActions.Deletes.Count, Is.EqualTo(2));
            Assert.That(applyEditActions.Updates.Single(x => Convert.ToInt32(x.Attributes["OBJECTID"]) == 2).Attributes["UpdateMe"],
                Is.EqualTo(contributions.Single(x => Convert.ToInt32(x.Attributes["OBJECTID"]) == 2).Attributes["UpdateMe"]));
        }
    }
    
    [TestFixture]
    public class ApplyEditActionForCopyTests {
        [Test]
        public void ActionIsNullIfContributionsAreEmpty()
        {
            var buildings = new List<FeatureAction>
                {
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"PropertyId", 3}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"PropertyId", 4}
                        }),
                    new FeatureAction(new Dictionary<string, object>
                        {
                            {"PropertyId", 5}
                        }),
                };

            var command = new CreateApplyEditActionsForCopyCommand(0, buildings);
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual.Total, Is.EqualTo(3));
        }

        [Test]
        public void ActionIsNullIfListsAreEmpty()
        {
            var command = new CreateApplyEditActionsForCopyCommand(0, new List<FeatureAction>());
            var actual = CommandExecutor.ExecuteCommand(command);

            Assert.That(actual, Is.Null);
        }
    }

}