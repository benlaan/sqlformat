using System;
using System.Linq;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestCreateTable
    {
        [Test]
        public void One_Int_Column()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>("create table Test ( id int )").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("id", statement.Fields[0].Name);
            Assert.AreEqual("int", statement.Fields[0].Type.Name);
        }

        [Test]
        public void One_Column_With_Owned_Table_Name()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>("create table dbo.Test ( id int )").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("dbo.Test", statement.TableName);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("id", statement.Fields[0].Name);
            Assert.AreEqual("int", statement.Fields[0].Type.Name);
        }

        [Test]
        public void One_Column_With_Multi_Scope_Table_Name()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>("create table SomeDatabase.dbo.Schema.Test ( id int )").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("SomeDatabase.dbo.Schema.Test", statement.TableName);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("id", statement.Fields[0].Name);
            Assert.AreEqual("int", statement.Fields[0].Type.Name);
        }

        [Test]
        public void Multiple_Int_Columns()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int, 
                    id2 int, 
                    id3 int 
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);
            Assert.AreEqual(3, statement.Fields.Count);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(String.Format("id{0}", i + 1), statement.Fields[i].Name);
                Assert.AreEqual("int", statement.Fields[i].Type.Name);
            }
        }

        [Test]
        public void Multiple_Int_Columns_With_Escaped_Names()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table [dbo].[Test]
                ( 
                    [id1] int, 
                    id2 int, 
                    [id3] int 
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("[dbo].[Test]", statement.TableName);
            Assert.AreEqual(3, statement.Fields.Count);

            var expected = new[]
            {
                new FieldDefinition { Name = "[id1]", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition { Name = "id2", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition { Name = "[id3]", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void Int_Column_With_Nullability()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int primary key, 
                    id2 int null, 
                    id3 int not null 
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition { Name = "id2", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition { Name = "id3", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.NotNullable },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void Columns_With_Complex_Data_Types()
        {
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int primary key, 
                    id2 varchar(10), 
                    id3 varchar not null, 
                    id4 decimal(10,2) not null,
                    id5 [decimal](25, 3) null,
                    id6 varchar(max) null
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition { Name = "id1", Type = new SqlType("int"), IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition { Name = "id2", Type = new SqlType("varchar") { Length = 10 } , IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition { Name = "id3", Type = new SqlType("varchar"), IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition { Name = "id4", Type = new SqlType("decimal") { Length = 10, Scale = 2 }, IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition { Name = "id5", Type = new SqlType("decimal") { Length = 25, Scale = 3 }, IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition { Name = "id6", Type = new SqlType("varchar") { Max = true }, IsPrimaryKey = false, Nullability = Nullability.Nullable },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void Primary_Key_As_Constraint()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int           not null, 
                    id2 varchar(10), 
                    id3 varchar       not null, 
                    id4 [decimal](10,2) not null, 
            
                    constraint [PK_Name] primary key clustered ( [id1] ASC )
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = true, Nullability = Nullability.NotNullable },
                new FieldDefinition { Name = "id2", Type = new SqlType( "varchar" ) { Length = 10 }, IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition { Name = "id3", Type = new SqlType( "varchar" ), IsPrimaryKey = false, Nullability = Nullability.NotNullable },
                new FieldDefinition { Name = "id4", Type = new SqlType( "decimal" ) { Length = 10, Scale = 2 }, IsPrimaryKey = false, Nullability = Nullability.NotNullable },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void Int_Column_With_Identity()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 [int] IDENTITY(100, 1) NOT NULL,
                    id2 varchar(10)
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition()
                {
                    Name = "id1",
                    Type = new SqlType("int"),
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable,
                    Identity = new Identity { Start = 100, Increment = 1 }
                },
                new FieldDefinition()
                {
                    Name = "id2", Type = new SqlType("varchar") { Length = 10 }, IsPrimaryKey = false, Nullability = Nullability.Nullable
                },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void Int_Column_With_Identity_After_Not_Null()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 [int] IDENTITY(100, 1) NOT NULL,
                    id2 varchar(10)
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition()
                {
                    Name = "id1",
                    Type = new SqlType( "int" ),
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable,
                    Identity = new Identity { Start = 100, Increment = 1 }
                },
                new FieldDefinition()
                {
                    Name = "id2", Type = new SqlType("varchar") { Length = 10 }, IsPrimaryKey = false, Nullability = Nullability.Nullable
                },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void VarChar_Column_With_Collation()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int,
                    id2 varchar(10) collate Latin1_General_CI_AS null
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
                new FieldDefinition()
                {
                    Name = "id2",
                    Type = new SqlType("varchar") { Length = 10, Collation = "Latin1_General_CI_AS" } ,
                    IsPrimaryKey = false,
                    Nullability = Nullability.Nullable
                },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void VarChar_Column_With_Constraint()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int,
                    id2 bit,
                    id3 varchar(10) constraint [Name] default ((1)) not null
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
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
                    Type = new SqlType("varchar") { Length = 10 },
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable
                },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void VarChar_Column_With_Constraint_As_Complex_Expressions()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test
                (
                    id1 int,
                    id2 bit,
                    id3 uniqueidentifier constraint [Name] default (newid()) not null,
                    id4 varchar(10) constraint [Name] default ('Hello') not null,
                    id5 varchar(10) default (newid()) not null
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);

            var expected = new[]
            {
                new FieldDefinition { Name = "id1", Type = new SqlType( "int" ), IsPrimaryKey = false, Nullability = Nullability.Nullable },
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
                    Type = new SqlType( "uniqueidentifier" ),
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable
                },
                new FieldDefinition()
                {
                    Name = "id4",
                    Type = new SqlType("varchar") { Length = 10 },
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable
                },
                new FieldDefinition()
                {
                    Name = "id5",
                    Type = new SqlType("varchar") { Length = 10 },
                    IsPrimaryKey = false,
                    Nullability = Nullability.NotNullable
                },
            };

            Assert.AreEqual(expected, statement.Fields);
        }

        [Test]
        public void VarChar_Column_With_Calculated_Value()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                create table Test 
                ( 
                    id1 int,
                    id2 bit,
                    calcField AS
                        CASE WHEN [IsValid] = 1
                              AND ISNULL([EffectiveFrom], GETDATE()) <= GETDATE()
                              AND ([EffectiveTo] IS NULL OR [EffectiveTo] >= GETDATE()) THEN 1
                        ELSE 0
                        END
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("Test", statement.TableName);
            Assert.AreEqual(3, statement.Fields.Count);

            FieldDefinition calcField = statement.Fields.Last();
            Assert.AreEqual("calcField", calcField.Name);
            Assert.IsNull(calcField.Type);
            Assert.AreEqual(typeof(CaseWhenExpression), calcField.CalculatedValue.GetType());
        }

        [Test]
        public void Create_Table_With_Not_Null_After_Default()
        {
            // Setup
            var statement = ParserFactory.Execute<CreateTableStatement>(@"

                CREATE TABLE [dbo].[Data] (
                    [Id] UNIQUEIDENTIFIER CONSTRAINT [DF_Data_New_SequentialId] DEFAULT (newsequentialId()) NOT NULL,
                    [Name] VARCHAR(10)
                )"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("[dbo].[Data]", statement.TableName);
            Assert.AreEqual(2, statement.Fields.Count);

            FieldDefinition idField = statement.Fields.First();
            Assert.AreEqual("[Id]", idField.Name);
            Assert.AreEqual("UNIQUEIDENTIFIER", idField.Type.Name);
            Assert.AreEqual(Nullability.NotNullable, idField.Nullability);
        }
    }
}
