using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Newtonsoft.Json;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Tests.Models {

    [TestFixture]
    public class EditContainerTests {
        [Test]
        public void CountsEditsAndDeletesSeparately()
        {
            var container = new EditContainer(new[]
                {
                    new ApplyEditActions
                        {
                            Adds = new Collection<FeatureAction>
                                {
                                    new FeatureAction(null)
                                },
                            Deletes = new Collection<int>
                                {
                                    1,
                                    3,
                                    4,
                                    5,
                                    6,
                                    7,
                                    8
                                },
                            Updates = new Collection<FeatureAction>
                                {
                                    new FeatureAction(null),
                                    new FeatureAction(null)
                                }
                        }
                });

            Assert.That(container.Total, Is.EqualTo(3));
        }

        [Test]
        public void CountsEditsAndDeletesSeparatelyForMultiple()
        {
            var container = new EditContainer(new[]
                {
                    new ApplyEditActions
                        {
                            Adds = new Collection<FeatureAction>
                                {
                                    new FeatureAction(null)
                                },
                            Deletes = new Collection<int>
                                {
                                    1,
                                    3,
                                    4,
                                    5,
                                    6,
                                    7,
                                    8
                                },
                            Updates = new Collection<FeatureAction>
                                {
                                    new FeatureAction(null),
                                    new FeatureAction(null)
                                }
                        },
                    new ApplyEditActions
                        {
                            Adds = new Collection<FeatureAction>
                                {
                                    new FeatureAction(null)
                                },
                            Deletes = new Collection<int>
                                {
                                    1,
                                    3,
                                    4,
                                    5,
                                    6,
                                    7,
                                    8
                                },
                            Updates = new Collection<FeatureAction>
                                {
                                    new FeatureAction(null),
                                    new FeatureAction(null)
                                }
                        }
                });

            Assert.That(container.Total, Is.EqualTo(6));
        }

        [Test]
        public void IsZeroWhenThereAreNoAddsOrUpdates()
        {
            var container = new EditContainer(new[]
                {
                    new ApplyEditActions
                        {
                            Adds = new Collection<FeatureAction>(),
                            Deletes = new Collection<int>
                                {
                                    1,
                                    3,
                                    4,
                                    5,
                                    6,
                                    7,
                                    8
                                },
                            Updates = new Collection<FeatureAction>()
                        },
                    new ApplyEditActions
                        {
                            Adds = new Collection<FeatureAction>(),
                            Deletes = new Collection<int>
                                {
                                    1,
                                    3,
                                    4,
                                    5,
                                    6,
                                    7,
                                    8
                                },
                            Updates = new Collection<FeatureAction>()
                        }
                });

            Assert.That(container.Total, Is.EqualTo(0));
        }

        [Test]
        public void JsonSerializesLikeEsriSample()
        {
            // f=json&updates=%5B%7B%22
            const string esriJson =
                @"[{""id"":0,""adds"":[{""geometry"":{""x"":1.0,""y"":1.0},""attributes"":{""PropertyId"":1}},{""geometry"":{""x"":2.0,""y"":2.0},""attributes"":{""PropertyId"":2}}],""updates"":[{""geometry"":{""x"":0.0,""y"":0.0},""attributes"":{""OBJECTID"":0,""PropertyId"":0}}],""deletes"":[19,23]},{""id"":1,""deletes"":[34,44]}]";

            var container = new[]
                {
                    new ApplyEditActions
                        {
                            Id = 0,
                            Adds = new Collection<FeatureAction>
                                {
                                    new FeatureAction(new Dictionary<string, object>
                                        {
                                            {"PropertyId", 1}
                                        }, new Point(1, 1)),
                                    new FeatureAction(new Dictionary<string, object>
                                        {
                                            {"PropertyId", 2}
                                        }, new Point(2, 2))
                                },
                            Deletes = new Collection<int>
                                {
                                    19,
                                    23
                                },
                            Updates = new Collection<FeatureAction>
                                {
                                    new FeatureAction(new Dictionary<string, object>
                                        {
                                            {"OBJECTID", 0},
                                            {"PropertyId", 0}
                                        }, new Point(0, 0))
                                }
                        },
                    new ApplyEditActions
                        {
                            Id = 1,
                            Deletes = new Collection<int>
                                {
                                    34,
                                    44
                                }
                        }
                };

            var actual = JsonConvert.SerializeObject(container);

            Assert.That(actual, Is.EqualTo(esriJson));
        }
    }

}