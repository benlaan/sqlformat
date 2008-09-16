using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

using Laan.SQL.Parser;

namespace Laan.SQLParser.Test
{
    [TestFixture]
    public class TestCreateNonClusteredIndex
    {
        [Test]
        public void Test_Can_Read_Non_Clusetered_Index()
        {
            // Setup
            var statement = ParserFactory.Execute( @"

                create nonclustered index [ix_id] on [dbo].[Test] ([otherid])
                "
            );

            // Verify outcome
            Assert.IsNull( statement );

            // Exercise

            // Verify outcome

        }
    }
}
