using System;
using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestBlockStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Begin_End_Block()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "BEGIN SELECT TOP 20 Field1, Field2 FROM dbo.Table T END" );

            // Verify outcome
            var expected = new[]
            {
               @"BEGIN",
                "",
                "    SELECT TOP 20",
                "        Field1,",
                "        Field2",
                "",
                "    FROM dbo.Table T",
                "",
                "END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Begin_End_Block_With_If_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "IF @A > 1 BEGIN SELECT TOP 20 Field1, Field2 FROM dbo.Table T END" );

            // Verify outcome
            var expected = new[]
            {
               @"IF @A > 1",
               @"BEGIN",
                "",
                "    SELECT TOP 20",
                "        Field1,",
                "        Field2",
                "",
                "    FROM dbo.Table T",
                "",
                "END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Begin_End_Block_With_If_Else_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "IF @A > 1 BEGIN SELECT TOP 20 Field1, Field2 FROM dbo.Table T END ELSE BEGIN SELECT @ID END" );

            // Verify outcome
            var expected = new[]
            {
               @"IF @A > 1",
               @"BEGIN",
                "",
                "    SELECT TOP 20",
                "        Field1,",
                "        Field2",
                "",
                "    FROM dbo.Table T",
                "",
                "END",
                "ELSE",
                "BEGIN",
                "",
                "    SELECT @ID",
                "",
                "END"
            };

            Compare( actual, expected );
        }
    }
}
