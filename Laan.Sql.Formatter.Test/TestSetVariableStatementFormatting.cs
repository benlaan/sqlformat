using System;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestSetVariableStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Create_Formatting_Engine()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise

            // Verify outcome
            Assert.IsNotNull(sut);
        }

        [Test]
        public void Can_Format_Simple_Set_Variable_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @variable = 10");

            // Verify outcome
            var expected = new[]
            {
                "SET @variable = 10"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_String_Value()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @name = 'John Doe'");

            // Verify outcome
            var expected = new[]
            {
                "SET @name = 'John Doe'"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Expression()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @total = @value1 + @value2");

            // Verify outcome
            var expected = new[]
            {
                "SET @total = @value1 + @value2"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Function_Call()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @currentDate = GETDATE()");

            // Verify outcome
            var expected = new[]
            {
                "SET @currentDate = GETDATE()"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Long_Select_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @Count = (SELECT COUNT(*) FROM dbo.Users WHERE IsActive = 1 AND CreatedDate > '2025-01-01' AND Role = 'Admin')");

            // Verify outcome
            var expected = new[]
            {
                "SET @Count = (",
                "",
                "    SELECT COUNT(*)",
                "",
                "    FROM dbo.Users",
                "",
                "    WHERE IsActive = 1",
                "      AND CreatedDate > '2025-01-01'",
                "      AND Role = 'Admin'",
                "",
                ")"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Short_Select()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @result = (SELECT MAX(Amount) FROM dbo.Orders WHERE Status = 'Active')");

            // Verify outcome
            var expected = new[]
            {
                "SET @result = (",
                "",
                "    SELECT MAX(Amount)",
                "    FROM dbo.Orders",
                "    WHERE Status = 'Active'",
                "",
                ")"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Terminator()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @value = 100;");

            // Verify outcome
            var expected = new[]
            {
                "SET @value = 100;"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Null_Value()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @variable = NULL");

            // Verify outcome
            var expected = new[]
            {
                "SET @variable = NULL"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Case_Expression()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("SET @status = CASE WHEN @value > 0 THEN 'Positive' ELSE 'NonPositive' END");

            // Verify outcome
            var expected = new[]
            {
                "SET @status = CASE WHEN @value > 0 THEN 'Positive' ELSE 'NonPositive' END"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Arithmetic_Expression()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("set @result = (@value1 * @value2) + (@value3 - @value4)");

            // Verify outcome
            var expected = new[]
            {
                "SET @result = (@value1 * @value2) + (@value3 - @value4)"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Set_Variable_With_Convert_Function()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("set @dateValue = CONVERT(DATE, '2026-03-19', 120)");

            // Verify outcome
            var expected = new[]
            {
                "SET @dateValue = CONVERT(DATE, '2026-03-19', 120)"
            };

            Compare(actual, expected);
        }
    }
}
