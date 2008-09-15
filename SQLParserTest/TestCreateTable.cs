using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestCreateTable
    {
        [Test]
        public void Test_One_Int_Column()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>( "create table Test ( id int )" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "id", statement.Fields[ 0 ].Name );
            Assert.AreEqual( "int", statement.Fields[ 0 ].Type.Name );
        }

        [Test]
        public void Test_One_Column_With_Owned_Table_Name()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>( "create table dbo.Test ( id int )" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "dbo.Test", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "id", statement.Fields[ 0 ].Name );
            Assert.AreEqual( "int", statement.Fields[ 0 ].Type.Name );
        }

        [Test]
        public void Test_Multiple_Int_Columns()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int, 
                    id2 int, 
                    id3 int 
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );
            Assert.AreEqual( 3, statement.Fields.Count );

            for ( int i = 0; i < 3; i++ )
            {
                Assert.AreEqual( String.Format( "id{0}", i + 1 ), statement.Fields[ i ].Name );
                Assert.AreEqual( "int", statement.Fields[ i ].Type.Name );
            }
        }

        [Test]
        public void Test_Multiple_Int_Columns_With_Escaped_Names()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table [dbo].[Test]
                ( 
                    [id1] int, 
                    id2 int, 
                    [id3] int 
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
            Assert.AreEqual( 3, statement.Fields.Count );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "[id1]", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "id2", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "[id3]", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
            };

            CollectionAssert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_Int_Column_With_Nullability()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int primary key, 
                    id2 int null, 
                    id3 int not null 
                )" 
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id2", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "id3", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.NotNullable },
            };

            CollectionAssert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_Columns_With_Complex_Data_Types()
        {
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int primary key, 
                    id2 varchar(10), 
                    id3 varchar not null, 
                    id4 decimal(10,2) not null,
                    id5 [decimal](25, 3) null 
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id2", Type = new SqlType( "varchar", 10 ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "id3", Type = new SqlType( "varchar" ), IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id4", Type = new SqlType( "decimal", 10, 2) , IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id5", Type = new SqlType( "decimal", 25, 3) , IsPrimaryKey = false, Nullability = Nullability.Nullable },
            };

            CollectionAssert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_Primary_Key_As_Constraint()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int           not null, 
                    id2 varchar(10), 
                    id3 varchar       not null, 
                    id4 [decimal](10,2) not null, 
            
                    constraint [PK_Name] primary key clustered ( [id1] ASC )
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id2", Type = new SqlType( "varchar", 10 ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() { Name = "id3", Type = new SqlType( "varchar" ), IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition() { Name = "id4", Type = new SqlType( "decimal", 10,2) , IsPrimaryKey = false, Nullability = Nullability.NotNullable },
            };

            CollectionAssert.AreElementsEqual( expected, statement.Fields );
        }
    }
}
