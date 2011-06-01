using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestExpressionFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Select_With_Short_Case_Statement()
        {
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT CanInline = CASE A.field WHEN 1 THEN 'Y' ELSE 'N' END, OtherID FROM dbo.Table"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    CanInline = CASE A.field WHEN 1 THEN 'Y' ELSE 'N' END,",
                "    OtherID",
                "",
                "FROM dbo.Table"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Only_One_Field_As_Case()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT CASE A.field WHEN 1 THEN 'Y' WHEN @A + 2 THEN 'N' WHEN @A / 4 THEN 'X' ELSE 'U' END"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    CASE A.field",
                "        WHEN 1 THEN 'Y'",
                "        WHEN @A + 2 THEN 'N'",
                "        WHEN @A / 4 THEN 'X'",
                "    ELSE",
                "        'U'",
                "    END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Simple_CaseSwitch()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT A.Field1, CASE A.field WHEN 1 THEN 'Y' WHEN @A + 2 THEN 'N' WHEN @A / 4 THEN 'X' ELSE 'U' END"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    A.Field1,",
                "    CASE A.field",
                "        WHEN 1 THEN 'Y'",
                "        WHEN @A + 2 THEN 'N'",
                "        WHEN @A / 4 THEN 'X'",
                "    ELSE",
                "        'U'",
                "    END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Simple_Case()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT A.Field1, CASE WHEN A.Field1 = 1 THEN 'Y' WHEN A.Field2 <> 2 THEN 'N' WHEN A.Field4 = 3 THEN 'M' ELSE 'U'   END"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    A.Field1,",
                "    CASE",
                "        WHEN A.Field1 = 1 THEN 'Y'",
                "        WHEN A.Field2 <> 2 THEN 'N'",
                "        WHEN A.Field4 = 3 THEN 'M'",
                "    ELSE",
                "        'U'",
                "    END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Case_When_With_Nested_Case_When()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT CASE WHEN A >10 THEN 'Ten' WHEN A>1000    
                THEN CASE WHEN A> 10 THEN 'Ten' WHEN A>1000     
                THEN 'Thousand' ELSE 'Lots' END ELSE 'Lots' END"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT",
                "    CASE",
                "        WHEN A > 10 THEN 'Ten'",
                "        WHEN A > 1000 THEN ",
                "            CASE",
                "                WHEN A > 10 THEN 'Ten'",
                "                WHEN A > 1000 THEN 'Thousand'",
                "            ELSE",
                "                'Lots'",
                "            END",
                "    ELSE",
                "        'Lots'",
                "    END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Case_When_With_Nested_Case_Switch()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT CASE WHEN A.ThisField = 1 THEN 'One' WHEN A = 2 THEN CASE A.Field 
                WHEN 2 THEN 'Two' WHEN 3 THEN 'Three' END WHEN A > 3 THEN 'Many' END"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT",
                "    CASE",
                "        WHEN A.ThisField = 1 THEN 'One'",
                "        WHEN A = 2 THEN CASE A.Field WHEN 2 THEN 'Two' WHEN 3 THEN 'Three' END",
                "        WHEN A > 3 THEN 'Many'",
                "    END",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Case_Switch_With_Nested_Case_Switch()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT CASE A.ThisField WHEN 1 THEN 'One' WHEN 2 THEN CASE A.Field 
                WHEN 2 THEN 'Two' WHEN 3 THEN 'Three' END WHEN A > 3 THEN 'Many' END"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT",
                "    CASE A.ThisField",
                "        WHEN 1 THEN 'One'",
                "        WHEN 2 THEN CASE A.Field WHEN 2 THEN 'Two' WHEN 3 THEN 'Three' END",
                "        WHEN A > 3 THEN 'Many'",
                "    END",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Function()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"SELECT dbo.SomeFunction(A.Value,A.Other) FROM dbo.Ark A"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT dbo.SomeFunction(A.Value, A.Other)",
                "FROM dbo.Ark A"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Function_With_Long_Param_Signature()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT dbo.SomeFunction(A.AReallyLongField,A.AReallyLongFieldAgain, 
                A.AReallyLongFieldOneMoreTime) FROM dbo.Ark A"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT",
                "    dbo.SomeFunction(",
                "        A.AReallyLongField,",
                "        A.AReallyLongFieldAgain,",
                "        A.AReallyLongFieldOneMoreTime",
                "    )",
                "FROM dbo.Ark A"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Exists_Function()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( 
                @"SELECT CASE WHEN EXISTS( SELECT 1 FROM dbo.Notes WHERE A=1) THEN 1 ELSE 0 END FROM dbo.Ark A"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT",
                "    CASE",
                "        WHEN EXISTS(",
                "",
                "            SELECT 1",
                "            FROM dbo.Notes",
                "            WHERE A = 1",
                "",
                "        ) THEN 1",
                "    ELSE",
                "        0",
                "    END",
                "FROM dbo.Ark A"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Not_Exists_Function()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(
                @"SELECT CASE WHEN NOT EXISTS( SELECT 1 FROM dbo.Notes WHERE A=1) THEN 1 ELSE 0 END FROM dbo.Ark A"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT",
                "    CASE",
                "        WHEN NOT EXISTS(",
                "",
                "            SELECT 1",
                "            FROM dbo.Notes",
                "            WHERE A = 1",
                "",
                "        ) THEN 1",
                "    ELSE",
                "        0",
                "    END",
                "FROM dbo.Ark A"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Between_Expression()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(
                @"SELECT * FROM dbo.Table A WHERE A.Field BETWEEN 10 AND (SELECT TOP 1 ID FROM Keys )"
            );

            // Verify outcome
            var expected = new[]
            {
                "SELECT *",
                "FROM dbo.Table A",
                "WHERE A.Field BETWEEN 10 AND (SELECT TOP 1 ID FROM Keys)",
            };

            Compare( actual, expected );
        }
    }
}
