using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestGrantStatementParser
    {
        [Test]
        [TestCase( "SELECT" ), TestCase( "INSERT" ), TestCase( "UPDATE" ), TestCase( "DELETE" ), TestCase( "ALL" )]
        public void TestGrantOperation( string operation )
        {
            // Exercise
            var statement = ParserFactory.Execute<GrantStatement>(
                String.Format( "GRANT {0} ON [dbo].[SomeTable] TO [SomeUser]", operation )
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[SomeTable]", statement.TableName );
            Assert.AreEqual( operation, statement.Operation );
            Assert.AreEqual( "[SomeUser]", statement.Grantee );
        }
    }
}
