using System;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestExecStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Exec_Statement_Without_Arguments()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec SomeFunc";
            
            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
               "EXEC SomeFunc",
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Exec_Statement_With_Implicit_Arguments_That_Dont_Require_Wrapping()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec SomeFunc 10, 'Ben', '2016-12-05', 1";

            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
               "EXEC SomeFunc 10, 'Ben', '2016-12-05', 1",
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Exec_Statement_With_Explicit_Arguments_That_Dont_Require_Wrapping()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec SomeFunc @Id = 10, 
                @Name = 'Ben',   @BirthDate = '2016-12-05', 
                @Ready = 1";

            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
               "EXEC SomeFunc @Id = 10, @Name = 'Ben', @BirthDate = '2016-12-05', @Ready = 1",
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Exec_Statement_With_Implicit_Arguments_That_Do_Require_Wrapping()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec SomeFunc 10, 'Ben', '2016-12-05', 1, 'a093203jwjlsdflsldfkjhdkfsjdkfjshdkjfhskjdsdfoirDFsds23234534dfsd'";

            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
               "EXEC SomeFunc",
               "    10,",
               "    'Ben',",
               "    '2016-12-05',",
               "    1,",
               "    'a093203jwjlsdflsldfkjhdkfsjdkfjshdkjfhskjdsdfoirDFsds23234534dfsd'"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Exec_Statement_With_Explicit_Arguments_That_Do_Require_Wrapping()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec SomeFunc @Id =10, 
                @Name= 'Ben',   @BirthDate = '2016-12-05',
                @Ready = 1, @Raw = 'a093203jwjlsdflsldfkjhdkfsjdkfjshdkjfhskjdsdfoirDFsds23234534dfsd'";

            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
               "EXEC SomeFunc",
               "    @Id = 10,",
               "    @Name = 'Ben',",
               "    @BirthDate = '2016-12-05',",
               "    @Ready = 1,",
               "    @Raw = 'a093203jwjlsdflsldfkjhdkfsjdkfjshdkjfhskjdsdfoirDFsds23234534dfsd'"
            };

            Compare(actual, expected);
        }
    }
}