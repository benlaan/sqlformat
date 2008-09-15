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
            IStatement sut = ParserFactory.Execute( "create table Test ( id int )" );
            CreateTableStatement statement = sut as CreateTableStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "id", statement.Fields[ 0 ].Name );
            Assert.AreEqual( "int", statement.Fields[ 0 ].Type );
        }

        [Test]
        public void Test_Multiple_Int_Columns()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "create table Test ( id1 int, id2 int, id3 int )" );
            CreateTableStatement statement = sut as CreateTableStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );
            Assert.AreEqual( 3, statement.Fields.Count );

            for ( int i = 0; i < 3; i++ )
            {
                Assert.AreEqual( String.Format( "id{0}", i + 1 ), statement.Fields[ i ].Name );
                Assert.AreEqual( "int", statement.Fields[ i ].Type );
            }
        }

        [Test]
        public void Test_Column_With_Complex_Data_Type()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( @"
                create table Test 
                ( 
                    id1 int           primary key, 
                    id2 varchar(10), 
                    id3 varchar       not null, 
                    id4 decimal(10,2) not null 
                )" 
            );
            CreateTableStatement statement = sut as CreateTableStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = "int", IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id2", Type = "varchar(10)", IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "id3", Type = "varchar", IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id4", Type = "decimal(10,2)", IsPrimaryKey = false, Nullability = Nullability.NotNullable },
            };

            CollectionAssert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_Int_Column_With_Nullability()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "create table Test ( id1 int primary key, id2 int null, id3 int not null )" );
            CreateTableStatement statement = sut as CreateTableStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = "int", IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id2", Type = "int", IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "id3", Type = "int", IsPrimaryKey = false, Nullability = Nullability.NotNullable },
            };

            CollectionAssert.AreElementsEqual( expected, statement.Fields );
        }
    }
}
