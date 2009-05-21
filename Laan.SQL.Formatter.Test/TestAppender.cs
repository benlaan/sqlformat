using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

using NHibernate.Appender;
using log4net.Core;
using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace Laan.SQL.Formatter.Test
{
    public class TestAppender
    {
        private static void Compare( string actual, string[] formatted )
        {
            Assert.AreElementsEqual(
                formatted,
                actual.Split( new string[] { "\r\n" }, StringSplitOptions.None )
            );
        }
        
        private ParamBuilderFormatter _appender;

        [SetUp]
        public void Setup()
        {
            // Setup
            _appender = new ParamBuilderFormatter();
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
            var actual = _appender.GetFormattedSQL( "SELECT * FROM States" );
            LogManager.Shutdown();

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM States",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_With_Params()
        {
            // Exercise
            var actual = _appender.GetFormattedSQL( "SELECT * FROM States WHERE ID=@P1 AND Name<>@P2;@P1=20,@P2='SA'" );
            LogManager.Shutdown();

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM States",
                "",
                "WHERE ID = 20",
                "  AND Name <> 'SA'",
            };

            Compare( actual, expected );
        }
    }
}
