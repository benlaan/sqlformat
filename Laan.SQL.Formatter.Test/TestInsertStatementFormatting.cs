using System;

using MbUnit.Framework;

namespace Laan.SQL.Formatter.Test
{
    [TestFixture]
    [Ignore( "Not finished yet!" )]
    public class TestInsertStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Few_Column_Insert_Statement_Without_Column_Listing()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "Insert into dbo.Table VALUES (1, 'Hello World')" );

            // Verify outcome
            var expected = new[]
            {
               "INSERT INTO dbo.Table",
               "     VALUES (1, 'Hello World')"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Few_Column_Insert_Statement_With_Column_Listing()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "Insert into dbo.Table (ID, Greeting) VALUES (1, 'Hello World')" );

            // Verify outcome
            var expected = new[]
            {
               "INSERT INTO dbo.Table (ID, Greeting)",
               "     VALUES (1, 'Hello World')"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Many_Column_Insert_Statement_Without_Column_Listing()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                Insert into dbo.Table VALUES (1, 'Hello World', 500, 
                'A Long Text Description Goes Here', 250, '01/01/2000','01/01/2009')"
            );

            // Verify outcome
            var expected = new[]
            {
               "INSERT INTO dbo.Table",
               "     VALUES (1, 'Hello World', 500, 'A Long Text Description Goes Here', 250, '01/01/2000', '01/01/2009')"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Many_Column_Insert_Statement_With_Column_Listing()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                Insert into dbo.Table ( ID, Greeting, MaxiumumDaysBetweenEvents, Description, EffectiveFromDate, 
                EffectiveToDate ) VALUES (1, 'Hello World', 500, 
                'A Long Text Description Goes Here', 250, '01/01/2000','01/01/2009')"
            );

            // Verify outcome
            var expected = new[]
            {
               "INSERT INTO dbo.Table (",
               "    ID, Greeting, MaxiumumDaysBetweenEvents, Description, EffectiveFromDate, EffectiveToDate",
               "    )",
               "     VALUES (1, 'Hello World', 500, 'A Long Text Description Goes Here', 250, '01/01/2000', '01/01/2009')"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Insert_Statement_With_Column_Listing_And_Select_Values()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                Insert into dbo.Table ( ID, Greeting, MaxiumumDaysBetweenEvents, Description, EffectiveFromDate, 
                EffectiveToDate ) SELECT ID, Greeting, MaxiumumDaysBetweenEvents * 2, Description, EffectiveFromDate, 
                EffectiveToDate FROM dbo.Events WHERE IsCancelled = 1 "
            );

            // Verify outcome
            var expected = new[]
            {
               "INSERT INTO dbo.Table (",
               "    ID, Greeting, MaxiumumDaysBetweenEvents, Description, EffectiveFromDate, EffectiveToDate",
               "    )",
               "    SELECT",
               "        ID,",
               "        Greeting,",
               "        MaxiumumDaysBetweenEvents * 2,",
               "        Description,",
               "        EffectiveFromDate,",
               "        EffectiveToDate",
               "",
               "    FROM dbo.Events",
               "",
               "    WHERE IsCancelled = 1",

            };

            Compare( actual, expected );
        }
    }
}
