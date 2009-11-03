using System;
using System.Linq;

using MbUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestDeclareStatements
    {
        [Test]
        [ExpectedException( typeof( SyntaxException ), "DECLARE requires at least one variable declaration" )]
        public void Declare_Statement_With_No_Variables_Should_Fail()
        {
            // Setup
            var sql = "DECLARE";

            // Exercise
            ParserFactory.Execute<DeclareStatement>( sql );
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), "type missing for declaration of variable '@Variable'" )]
        public void Declare_Statement_With_One_Variable_Without_Type_Should_Fail()
        {
            // Setup
            var sql = "DECLARE @Variable";

            // Exercise
            ParserFactory.Execute<DeclareStatement>( sql );
        }

        [Test]
        public void Declare_Statement_With_One_Variable()
        {
            // Setup
            var sql = "DECLARE @Variable INT";

            // Exercise
            var statement = ParserFactory.Execute<DeclareStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Definitions.Count );
            Assert.AreEqual( "@Variable", statement.Definitions.First().Name );
            Assert.AreEqual( "INT", statement.Definitions.First().Type );
        }

        [Test]
        public void Declare_Statement_With_One_Variable_With_Default_Value()
        {
            // Setup
            var sql = "DECLARE @Variable INT = 20 * 2";

            // Exercise
            var statement = ParserFactory.Execute<DeclareStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Definitions.Count );

            VariableDefinition definition = statement.Definitions.First();
            Assert.AreEqual( "@Variable", definition.Name );
            Assert.AreEqual( "INT", definition.Type );
            Assert.AreEqual( typeof( OperatorExpression ), definition.DefaultValue.GetType() );
            Assert.AreEqual( "*", ( (OperatorExpression) definition.DefaultValue ).Operator );
        }

        [Test]
        public void Declare_Statement_With_Multiple_Variables()
        {
            // Setup
            var sql = "DECLARE @v1 INT, @v2 VARCHAR(50), @v3 DECIMAL(10,2)";

            // Exercise
            var statement = ParserFactory.Execute<DeclareStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Definitions.Count );

            var variables = new[] 
            { 
                new { Name = "@v1", Type = "INT" }, 
                new { Name = "@v2", Type = "VARCHAR(50)" },
                new { Name = "@v3", Type = "DECIMAL(10, 2)" } 
            };

            int index = 0;
            foreach ( var variable in variables )
            {
                VariableDefinition definition = statement.Definitions[ index++ ];
                Assert.AreEqual( variable.Name, definition.Name );
                Assert.AreEqual( variable.Type, definition.Type );
            }
        }

        [Test]
        public void Declare_Statement_With_Multiple_Variables_With_Default_Values()
        {
            // Setup
            var sql = "DECLARE @v1 INT = 20, @v2 VARCHAR(50) = 'Hello', @v3 DECIMAL(10,2) = 12.5 * @v1";

            // Exercise
            var statement = ParserFactory.Execute<DeclareStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Definitions.Count );

            var variables = new[] 
            { 
                new { Name = "@v1", Type = "INT",            DefaultValue = "20", ExpressionType = typeof(IdentifierExpression) }, 
                new { Name = "@v2", Type = "VARCHAR(50)",    DefaultValue = "'Hello'", ExpressionType = typeof(StringExpression) },
                new { Name = "@v3", Type = "DECIMAL(10, 2)", DefaultValue = "12.5 * @v1", ExpressionType = typeof(OperatorExpression) } 
            };

            int index = 0;
            foreach ( var variable in variables )
            {
                VariableDefinition definition = statement.Definitions[ index++ ];
                Assert.AreEqual( variable.Name, definition.Name );
                Assert.AreEqual( variable.Type, definition.Type );
                Assert.AreEqual( variable.ExpressionType, definition.DefaultValue.GetType() );
            }
        }
    }
}
