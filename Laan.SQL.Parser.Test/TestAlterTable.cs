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
            ).First();

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
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
        }

        [Test]
        public void Test_Add_Clustered_Primary_Key_With_Multiple_Columns()
        {
            // Exercise
            var statement = ParserFactory.Execute<AlterTableStatement>( @"

                alter table [dbo].[Test] 
                        add constraint [PK_Test] 
                        primary key clustered ( [id1],id2, id3, [Hello World] )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
            Assert.AreEqual( 4, statement.PrimaryKeys.Count );
            Assert.AreEqual( "[id1]", statement.PrimaryKeys[ 0 ] );
            Assert.AreEqual( "id2", statement.PrimaryKeys[ 1 ] );
            Assert.AreEqual( "id3", statement.PrimaryKeys[ 2 ] );
            Assert.AreEqual( "[Hello World]", statement.PrimaryKeys[ 3 ] );
        }

        [Test]
        public void Test_Add_Foreign_Key()
        {
            // Exercise
            var statement = ParserFactory.Execute<AlterTableStatement>( @"

                alter table [dbo].[Test] 
                add constraint [fk_test] foreign key ([otherID]) references [dbo].[OtherTable] ([id])
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
        }

        [Test]
        public void Test_Add_Unique_Non_Clustered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<AlterTableStatement>( @"

                ALTER TABLE [dbo].[Computers] 
                        ADD CONSTRAINT [IX_Computers] UNIQUE NONCLUSTERED ([MachineName])
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Computers]", statement.TableName );
        }
    }
}
