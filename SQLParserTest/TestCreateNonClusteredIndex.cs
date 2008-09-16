using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

using Laan.SQL.Parser;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestCreateNonClusteredIndex
    {
        [Test]
        public void Test_Can_Read_Non_Clusetered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateNonClusteredIndex>( @"

                create nonclustered index [ix_id] on [dbo].[Test] ([otherid])
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "[dbo].[Test]", statement.TableName );
            Assert.AreEqual( "[ix_id]", statement.ConstraintName );
            Assert.AreEqual( "[otherid]", statement.KeyName ); 
        }
    }
}
