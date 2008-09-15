using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using SQLParser;

namespace SQLParserTest
{
    [TestFixture]
    public class TestCreateTable
    {
        [Test]
        public void Test_One_Int_Column()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "create table { id int }" );
            CreateTableStatement statement = sut as CreateTableStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "id", statement.Fields[ 0 ].Name );
            Assert.AreEqual( "int", statement.Fields[ 0 ].Type );
        }

        [Test]
        public void Test_Multiple_Int_Columns()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "create table { id1 int, id2 int, id3 int }" );
            CreateTableStatement statement = sut as CreateTableStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Fields.Count );

            for ( int i = 0; i < 3; i++ )
            {
                Assert.AreEqual( String.Format("id{0}", i + 1), statement.Fields[ i ].Name );
                Assert.AreEqual( "int", statement.Fields[ i ].Type );
            }
        }
    }
}
