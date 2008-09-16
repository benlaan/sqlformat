using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestAlterTable
    {
        [Test]
        public void Test_Add_Clustered_Primary_Key()
        {
            // Exercise
            var statement = ParserFactory.Execute<AlterTableStatement>( @"

                alter table [dbo].[Test] 
                        add constraint [PK_Test] primary key clustered ( [id] )
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
        }

        [Test]
        public void Test_Add_Clustered_Primary_Key_With_No_Check()
        {
            // Exercise
            var statement = ParserFactory.Execute<AlterTableStatement>( @"

                alter table [dbo].[Test] with nocheck
                        add constraint [PK_Test] primary key clustered ( [id] )
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
        }

        [Test]
        public void Test_Add_Foreign_Key()
        {
            // Exercise
            var statement = ParserFactory.Execute<AlterTableStatement>( @"

                alter table [dbo].[Test] 
                add constraint [fk_test] foreign key ([otherID]) references [dbo].[OtherTable] ([id])
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
        }
    }
}
