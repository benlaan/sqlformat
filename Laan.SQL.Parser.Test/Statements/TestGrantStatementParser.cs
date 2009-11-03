using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gallio.Framework;
using MbUnit.Framework;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.SqlParser.Test
{
    [TestFixture]
    public class TestGrantStatementParser
    {
        [Test]
        [Row( "SELECT" ), Row( "INSERT" ), Row( "UPDATE" ), Row( "DELETE" ), Row( "ALL" )]
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
