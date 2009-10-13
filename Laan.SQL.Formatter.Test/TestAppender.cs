using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

using Laan.NHibernate.Appender;

namespace Laan.SQL.Formatter.Test
{
    public class TestAppender : BaseFormattingTest
    {
        private ParamBuilderFormatter _appender;

        [SetUp]
        public void Setup()
        {
            // Setup
            _appender = new ParamBuilderFormatter( new FormattingEngine() );
        }

        [Test]
        public void Can_Create_NHibernate_Appender()
        {
            // Verify outcome
            Assert.IsNotNull( _appender );
        }

        [Test]
        public void Can_Format_Without_Params()
        {
            // Exercise
            var actual = _appender.Execute( "SELECT * FROM States" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "FROM States",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_With_Params()
        {
            // Exercise
            var actual = _appender.Execute( "SELECT * FROM States WHERE ID=@P1 AND Name<>@P2;@P1=20,@P2='SA'" );

            // Verify outcome
            var expected = new[]
            {
                "SELECT *",
                "",
                "FROM States",
                "",
                "WHERE ID = 20",
                "  AND Name <> 'SA'",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_With_Guid_Param()
        {
            // Exercise
            var actual = _appender.Execute( @"
                SELECT * FROM States WHERE ID=@p0;@p0=5a3d68a3-f53b-40ce-8bdd-2f40a5d35ded" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "FROM States",
                "WHERE ID = '5A3D68A3-F53B-40CE-8BDD-2F40A5D35DED'",
            };

            Compare( actual, expected );
        }
    }
}
