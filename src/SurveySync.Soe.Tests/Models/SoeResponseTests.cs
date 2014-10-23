using System.Collections.Generic;
using NUnit.Framework;
using SurveySync.Soe.Models;
using SurveySync.Soe.Models.FeatureService;

namespace SurveySync.Soe.Tests.Models {

    [TestFixture]
    public class SoeResponseTests {
        [Test]
        public void CanCountDatabaseChanges()
        {
            var input = new[]
                {
                    new FeatureServiceEditResponse
                        {
                            Id = 1,
                            AddResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 1
                                        }
                                },
                            UpdateResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 2
                                        },
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 3
                                        }
                                },
                            DeleteResults = new List<FeatureServiceActionResponse>()
                        },
                    new FeatureServiceEditResponse
                        {
                            Id = 2,
                            AddResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 4
                                        }
                                },
                            UpdateResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 5
                                        }
                                },
                            DeleteResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 6
                                        }
                                }
                        }
                };

            var actual = new SoeResponse(input);

            Assert.That(actual.Successful, Is.True);
            Assert.That(actual.Deleted, Is.EqualTo(1));
            Assert.That(actual.Created, Is.EqualTo(2));
            Assert.That(actual.Updated, Is.EqualTo(3));
            Assert.That(actual.Message, Is.Empty);
        }

        [Test]
        public void CanHandleMixedUpdates()
        {
            // this should never happen (rollbackonerror) but why not have a test
            var input = new[]
                {
                    new FeatureServiceEditResponse
                        {
                            AddResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Error = new FeatureServiceError
                                                {
                                                    Description = "First Error.",
                                                    Code = 1
                                                }
                                        },
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 1
                                        }
                                },
                            UpdateResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Error = new FeatureServiceError
                                                {
                                                    Description = "Second Error.",
                                                    Code = 2
                                                }
                                        },
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 2
                                        }
                                },
                            DeleteResults = new List<FeatureServiceActionResponse>
                                {
                                    new FeatureServiceActionResponse
                                        {
                                            Error = new FeatureServiceError
                                                {
                                                    Description = "Third Error.",
                                                    Code = 3
                                                }
                                        },
                                    new FeatureServiceActionResponse
                                        {
                                            Success = true,
                                            ObjectId = 3
                                        }
                                }
                        }
                };

            var actual = new SoeResponse(input);

            Assert.That(actual.Successful, Is.False);
            Assert.That(actual.Updated, Is.EqualTo(1));
            Assert.That(actual.Deleted, Is.EqualTo(1));
            Assert.That(actual.Created, Is.EqualTo(1));
            Assert.That(actual.Message, Is.EqualTo("First Error. Second Error. Third Error."));
        }

        [Test]
        public void ModelCanUnionAllErrorMessages()
        {
            var input =new[] {new FeatureServiceEditResponse
                {
                    AddResults = new List<FeatureServiceActionResponse>
                        {
                            new FeatureServiceActionResponse
                                {
                                    Error = new FeatureServiceError
                                        {
                                            Description = "First Error.",
                                            Code = 1
                                        }
                                }
                        },
                    UpdateResults = new List<FeatureServiceActionResponse>
                        {
                            new FeatureServiceActionResponse
                                {
                                    Error = new FeatureServiceError
                                        {
                                            Description = "Second Error.",
                                            Code = 2
                                        }
                                }
                        },
                    DeleteResults = new List<FeatureServiceActionResponse>
                        {
                            new FeatureServiceActionResponse
                                {
                                    Error = new FeatureServiceError
                                        {
                                            Description = "Third Error.",
                                            Code = 3
                                        }
                                }
                        }
                }
            };

            var actual = new SoeResponse(input);

            Assert.That(actual.Successful, Is.False);
            Assert.That(actual.Updated, Is.EqualTo(0));
            Assert.That(actual.Deleted, Is.EqualTo(0));
            Assert.That(actual.Created, Is.EqualTo(0));
            Assert.That(actual.Message, Is.EqualTo("First Error. Second Error. Third Error."));
        }
    }

}