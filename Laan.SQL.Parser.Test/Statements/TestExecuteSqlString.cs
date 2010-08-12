using System;
using System.Linq;

using Laan.Sql.Parser.Entities;

using MbUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    public class ExecuteSqlStatement : Statement
    {
        public ExecuteSqlStatement()
        {
            
        }
    }

    [TestFixture]
    public class TestExecuteSqlString
    {
        [Test]
        public void Test_Can_Execute_Simple_Sql_String()
        {
          //  // Exercise
          //  var statement = ParserFactory.Execute<ExecuteSqlStatement>( @"
          //      sp_execute ( 'SELECT * FROM Table WHERE ID=@P1 AND Name=@P2',@P1 INT, @P2 VARCHAR(10),'@P1=20,@P2='Users'')
		        //"
          //  ).First();

          //  // Verify outcome
          //  Assert.IsNotNull( statement );
          //  Assert.AreEqual( "[dbo].[Computers]", "" );
        }
    }
}
