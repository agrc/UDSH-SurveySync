using System.Linq;
using CommandPattern;
using NUnit.Framework;
using SurveySync.Soe.Commands.Sql;

namespace SurveySync.Soe.Tests.Commands {

    public class SqlWhereClauseTests {
        [TestFixture]
        public class ComposeInSetQueryCommandTestsArcMap {
            [Test]
            public void SetOfIntegers()
            {
                var set = new[] {1, 2, 3, 4, 5};
                var command = new ComposeInSetQueryCommand("field", set.Cast<object>());
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.Not.Empty);
                Assert.That(actual, Is.EqualTo("field in (1,2,3,4,5)"));
            }

            [Test]
            public void SetOfStringsAreQuoted()
            {
                var set = new[] {"1", "2", "3", "4", "5"};
                var command = new ComposeInSetQueryCommand("field", set);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.Not.Empty);
                Assert.That(actual, Is.EqualTo("field in (\"1\",\"2\",\"3\",\"4\",\"5\")"));
            }
            [Test]
            public void ReturnNullWithEmptyIntSet()
            {
                var set = new int[0];
                var command = new ComposeInSetQueryCommand("field", set.Cast<object>());
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Null);
            }

            [Test]
            public void ReturnNullWithEmptyStringSet()
            {
                var set = new string[1];
                var command = new ComposeInSetQueryCommand("field", set);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Null);
            }
        }

        [TestFixture]
        public class ComposeInSetQueryCommandTestsTsql {
            [Test]
            public void MultipleReturnFieldsAndSet()
            {
                var set = new[] {1, 2, 3, 4, 5};
                var command = new ComposeInSetQueryCommand("table", "field", new[] {"return", "fields"},
                                                           set.Cast<object>());
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.Not.Empty);
                Assert.That(actual, Is.EqualTo("select return,fields from table where field in (1,2,3,4,5)"));
            }

            [Test]
            public void SetOfStringsAreQuoted()
            {
                var set = new[] { "1", "2", "3", "4", "5" };
                var command = new ComposeInSetQueryCommand("table", "field", new[] { "return", "fields" },
                                                           set);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.Not.Empty);
                Assert.That(actual, Is.EqualTo("select return,fields from table where field in (\"1\",\"2\",\"3\",\"4\",\"5\")"));
            }

            [Test]
            public void ReturnNullWithEmptyIntSet()
            {
                var set = new int[0];
                var command = new ComposeInSetQueryCommand("table", "field", new[] {"returnfields"},
                                                           set.Cast<object>());
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Null);
            }

            [Test]
            public void ReturnNullWithEmptyStringSet()
            {
                var set = new string[1];
                var command = new ComposeInSetQueryCommand("table", "field", new[] {"returnfields"},
                                                           set);
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Null);
            }

            [Test]
            public void SingleReturnFieldsAndMultipleSet()
            {
                var set = new[] {1, 2, 3, 4, 5};
                var command = new ComposeInSetQueryCommand("table", "field", new[] {"returnfields"},
                                                           set.Cast<object>());
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.Not.Empty);
                Assert.That(actual, Is.EqualTo("select returnfields from table where field in (1,2,3,4,5)"));
            }

            [Test]
            public void SingleReturnFieldsAndSet()
            {
                var set = new[] {1};
                var command = new ComposeInSetQueryCommand("table", "field", new[] {"returnfields"},
                                                           set.Cast<object>());
                var actual = CommandExecutor.ExecuteCommand(command);

                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.Not.Empty);
                Assert.That(actual, Is.EqualTo("select returnfields from table where field in (1)"));
            }
        }
    }

}