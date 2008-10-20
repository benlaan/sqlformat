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
        public void Test_One_Column_With_Multi_Scope_Table_Name()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>( "create table SomeDatabase.dbo.Schema.Test ( id int )" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "SomeDatabase.dbo.Schema.Test", statement.TableName );
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

            Assert.AreElementsEqual( expected, statement.Fields );
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

            Assert.AreElementsEqual( expected, statement.Fields );
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

            Assert.AreElementsEqual( expected, statement.Fields );
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

            Assert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_Int_Column_With_Identity()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 [int] IDENTITY(100, 1) NOT NULL,
                    id2 varchar(10)
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() 
                { 
                    Name = "id1", 
                    Type = new SqlType( "int" ), 
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable, 
                    Identity = new Identity() { Start = 100, Increment = 1 } 
                },
                new FieldDefinition() 
                { 
                    Name = "id2", Type = new SqlType( "varchar", 10 ), IsPrimaryKey = false, Nullability = Nullability.Nullable 
                },
            };

            Assert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_Int_Column_With_Identity_After_Not_Null()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 [int] NOT NULL IDENTITY(100, 1),
                    id2 varchar(10)
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() 
                { 
                    Name = "id1", 
                    Type = new SqlType( "int" ), 
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable, 
                    Identity = new Identity() { Start = 100, Increment = 1 } 
                },
                new FieldDefinition() 
                { 
                    Name = "id2", Type = new SqlType( "varchar", 10 ), IsPrimaryKey = false, Nullability = Nullability.Nullable 
                },
            };

            Assert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_VarChar_Column_With_Collation()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int,
                    id2 varchar(10) collate Latin1_General_CI_AS null
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() 
                { 
                    Name = "id2", 
                    Type = new SqlType( "varchar", 10 ) { Collation = "Latin1_General_CI_AS" } , 
                    IsPrimaryKey = false, 
                    Nullability = Nullability.Nullable 
                },
            };

            Assert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_VarChar_Column_With_Constraint()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int,
                    id2 bit,
                    id3 varchar(10) not null constraint [Name] default ((1))
                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() 
                { 
                    Name = "id2", 
                    Type = new SqlType( "bit" ),
                    IsPrimaryKey = false, 
                    Nullability = Nullability.Nullable 
                },
                new FieldDefinition() 
                { 
                    Name = "id3", 
                    Type = new SqlType( "varchar", 10 ),
                    IsPrimaryKey = false, 
                    Nullability = Nullability.NotNullable 
                },
            };

            Assert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test_VarChar_Column_With_Constraint_As_Complex_Expressions()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"

                create table Test 
                ( 
                    id1 int,
                    id2 bit,
                    id3 uniqueidentifier not null constraint [Name] default (newid()),
                    id4 varchar(10) not null constraint [Name] default ('Hello'),
                    id5 varchar(10) not null default (newid())

                )"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "Test", statement.TableName );

            var expected = new[] 
            { 
                new FieldDefinition() { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition() 
                { 
                    Name = "id2", 
                    Type = new SqlType( "bit" ),
                    IsPrimaryKey = false, 
                    Nullability = Nullability.Nullable 
                },
                new FieldDefinition() { 
                    Name = "id3", 
                    Type = new SqlType( "uniqueidentifier" ),
                    IsPrimaryKey = false, 
                    Nullability = Nullability.NotNullable 
                },
                new FieldDefinition() 
                { 
                    Name = "id4", 
                    Type = new SqlType( "varchar", 10 ),
                    IsPrimaryKey = false, 
                    Nullability = Nullability.NotNullable 
                },
                new FieldDefinition() 
                { 
                    Name = "id5", 
                    Type = new SqlType( "varchar", 10 ),
                    IsPrimaryKey = false, 
                    Nullability = Nullability.NotNullable
                },
            };

            Assert.AreElementsEqual( expected, statement.Fields );
        }

        [Test]
        public void Test()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>( @"
                CREATE TABLE Stage.[Parcels_Updates]
                (
                [ParcelID] [int] NOT NULL,
                [ParcelNumber] [varchar] (32) COLLATE Latin1_General_CI_AS NULL,
                [TransactionType] [int] NULL,
                [OwnerOrderNumber] [varchar] (16) COLLATE Latin1_General_CI_AS NULL,
                [IsWarehouser] [bit] NULL,
                [StartDateTime] [datetime] NULL,
                [ExpiryDate] [datetime] NULL,
                [TransportMode] [int] NULL,
                [IsFulfilled] [bit] NULL,
                [Responsibility] [nvarchar] (32) COLLATE Latin1_General_CI_AS NULL,
                [SpecialInstructions] [nvarchar] (64) COLLATE Latin1_General_CI_AS NULL,
                [CustomerID] [int] NULL,
                [SeasonID] [int] NULL,
                [CommodityID] [int] NULL,
                [BinGradeID] [int] NULL,
                [ToSiteID] [int] NULL,
                [FromSiteID] [int] NULL,
                [TotalWeightKilograms] [int] NULL,
                [VariancePercentage] [float] NULL,
                [VarianceAbsoluteKilograms] [int] NULL,
                [ReceiverShortName] [nvarchar] (32) COLLATE Latin1_General_CI_AS NULL,
                [ReceiverLongName] [nchar] (255) COLLATE Latin1_General_CI_AS NULL
                )
                "
            );

            // Exercise

            // Verify outcome

        }
    }
}
