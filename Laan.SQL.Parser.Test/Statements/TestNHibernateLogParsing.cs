using System;
using System.Collections.Generic;
using System.Linq;

using Laan.NHibernate.Appender;
using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestNHibernateLogParsing
    {
        /// <summary>
        /// This sql is captured via NHibernate.SQL logs
        /// </summary>
        [Test]
        public void Can_Parse_NHibernate_Log_Message_With_Parameters()
        {
            var sql = @"
                select TOP (@p0) T.Name
                from [Transaction]
                where (T.Code in (@p1, @p2));

                @p0 = 100 [Type: Int32 (0)], 
                @p1 = 'AA BB' [Type: String (4000)], 
                @p2 = 'CCC' [Type: String (4000)]
            ";

            // cleanup test data to match 'real' input, while allowing it to be readable above
            sql = sql.Replace(Environment.NewLine, " ");

            //  // Exercise
            ParameterSubstituter builder = new ParameterSubstituter();
            var splitSql = builder.UpdateParamsWithValues(sql);

            var statement = ParserFactory.Execute<SelectStatement>(splitSql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("[Transaction]", statement.From.First().Name);
            Assert.AreEqual("(100)", statement.Top.Expression.Value);
            Assert.AreEqual("(T.Code IN ('AA BB', 'CCC'))", statement.Where.Value);
        }

        /// <summary>
        /// This sql is captured via NHibernate.SQL logs
        /// </summary>
        [Test]
        public void Can_Parse_NHibernate_Log_Message_With_More_Than_Nine_Parameters()
        {
            var sql = @"
                select TOP (@p0) T.*
                from [Transaction]
                where (Code in (@p1, @p2, @p3 , @p4 , @p5 , @p6 , @p7 , @p8 , @p9 , @p10 , @p11 , @p12, @p13));

                @p0  = 100 [Type: Int32 (0)],   @p1  = 'A' [Type: String (1)],  @p2 =  'B' [Type: String (2)], 
                @p3  = 'C' [Type: String (3)],  @p4  = 'D' [Type: String (4)],  @p5 =  'E' [Type: String (5)], 
                @p6  = 'F' [Type: String (6)],  @p7  = 'G' [Type: String (7)],  @p8 =  'H' [Type: String (8)],
                @p9  = 'I' [Type: String (9)],  @p10 = 'J' [Type: String (10)], @p11 = 'K' [Type: String (11)], 
                @p12 = 'L' [Type: String (12)], @p13 = 'M' [Type: String (13)]
            ";

            // cleanup test data to match 'real' input, while allowing it to be readable above
            sql = sql.Replace(Environment.NewLine, " ");

            //  // Exercise
            ParameterSubstituter builder = new ParameterSubstituter();
            var splitSql = builder.UpdateParamsWithValues(sql);

            var statement = ParserFactory.Execute<SelectStatement>(splitSql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("[Transaction]", statement.From.First().Name);
            Assert.AreEqual("(100)", statement.Top.Expression.Value);
            Assert.AreEqual("(Code IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M'))", statement.Where.Value);
        }

        /// <summary>
        /// This sql is captured via NHibernate.SQL logs
        /// </summary>
        [Test]
        public void Can_Parse_NHibernate_Log_Message_With_DateTime_Parameter()
        {
            var sql = @"
                select T.Name
                from [Transaction]
                where Start > @p0;

                @p0 = 23/05/2011 2:51:54 PM [Type: DateTime (0)]
            ";

            // cleanup test data to match 'real' input, while allowing it to be readable above
            sql = sql.Replace(Environment.NewLine, " ");

            //  // Exercise
            ParameterSubstituter builder = new ParameterSubstituter();
            var splitSql = builder.UpdateParamsWithValues(sql);

            var statement = ParserFactory.Execute<SelectStatement>(splitSql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("[Transaction]", statement.From.First().Name);
            Assert.AreEqual("Start > '23/05/2011 2:51:54 PM'", statement.Where.Value);
        }

        /// <summary>
        /// This sql is captured via NHibernate.SQL logs
        /// </summary>
        [Test]
        public void Can_Parse_NHibernate_Log_Message_Without_Parameters()
        {
            var sql = @"
                select TOP (100) T.Name
                from [Transaction]
                where (T.Code in (1, 2))
            ";

            // cleanup test data to match 'real' input, while allowing it to be readable above
            sql = sql.Replace(Environment.NewLine, " ");

            // Exercise
            ParameterSubstituter builder = new ParameterSubstituter();
            var splitSql = builder.UpdateParamsWithValues(sql);

            var statement = ParserFactory.Execute<SelectStatement>(splitSql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("[Transaction]", statement.From.First().Name);
            Assert.AreEqual("(100)", statement.Top.Expression.Value);
            Assert.AreEqual("(T.Code IN (1, 2))", statement.Where.Value);
        }
    }
}
