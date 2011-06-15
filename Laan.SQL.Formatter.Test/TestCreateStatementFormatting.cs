using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestCreateStatementFormatting : BaseFormattingTest
    {

        [Test]
        public void Can_Format_Simple_Create_Index_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"CREATE UNIQUE NONCLUSTERED INDEX [IX_Sites_Code] ON dbo.Sites (Code) WITH (IGNORE_DUP_KEY = ON )" );

            // Verify outcome
            var expected = new[]
            {
                "CREATE UNIQUE NONCLUSTERED INDEX [IX_Sites_Code] ON dbo.Sites ( Code ) WITH ( IGNORE_DUP_KEY = ON )"
            };

            Compare( actual, expected );
        }

        /*
CREATE UNIQUE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvinceID_PostalCode] ON [Person].[Address] 
(
    [AddressLine1] ASC,
    [AddressLine2] ASC,
    [City] ASC,
    [StateProvinceID] ASC,
    [PostalCode] ASC
)

WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
         
         */

        [Test]
        public void Can_Format_Complex_Create_Index_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"CREATE UNIQUE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvinceID_PostalCode] ON [Person].[Address] ( [AddressLine1] ASC, [AddressLine2] DESC, [City] ASC, [StateProvinceID] DESC, [PostalCode] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]" );

            // Verify outcome
            var expected = new[]
            {
                "CREATE UNIQUE NONCLUSTERED INDEX [IX_Address_AddressLine1_AddressLine2_City_StateProvinceID_PostalCode] ON [Person].[Address]",
                "(",
                "    [AddressLine1],",
                "    [AddressLine2] DESC,",
                "    [City],",
                "    [StateProvinceID] DESC,",
                "    [PostalCode]",
                ")",
                "WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]"
            };

            Compare( actual, expected );

        }

        [Test]
        [Ignore("CreateStatementFormatter not implemented yet")]
        public void Can_Format_Simple_Create_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"

                create table Test 
                ( 
                    id1 [int] NOT NULL IDENTITY(100, 1),
                    id2 varchar(10)
                )" );

            // Verify outcome
            var expected = new[]
            {
                "CREATE TABLE [dbo].[Clients]",
                "(",
                "    [ClientID] [int] IDENTITY(1,1) NOT NULL,",
                "    [Name] [nvarchar](255) NOT NULL",
                ")"
            };

            Compare( actual, expected );
        }
    }
}
