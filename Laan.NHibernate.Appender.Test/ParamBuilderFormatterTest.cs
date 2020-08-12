using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Formatter;

using NUnit.Framework;

namespace Laan.NHibernate.Appender.Test
{
    [TestFixture]
    public class ParamBuilderFormatterTest
    {
        private const string sample =
            @"Batch commands:
command 0:INSERT INTO dbo.[Table] (Version, TypeID, Name, ShortName, Data, UserName, Created, Modified, IsDeleted, Id) VALUES " + 
            "(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9);@p0 = 1 [Type: Int32 (0)], @p1 = '0' [Type: String (255)], @p2 = " + 
            "'Reference' [Type: String (4000)], @p3 = NULL [Type: String (4000)], @p4 = 0 [Type: Int32 (0)], @p5 = 'auser' " + 
            "[Type: String (4000)], @p6 = 9/03/2012 11:39:50 AM [Type: DateTime (0)], @p7 = 9/03/2012 11:39:50 AM [Type: DateTime (0)], " + 
            "@p8 = False [Type: Boolean (0)], @p9 = d4a462ab-da14-46d2-9118-a00f00c037c6 [Type: Guid (0)]";

        private const string expected =
            @"INSERT INTO dbo.[Table] (
    Version, TypeID, Name, ShortName, Data, UserName, Created, Modified, IsDeleted, 
    Id
    )
    VALUES (1, '0', 'Reference', NULL, 0, 'auser', '9/03/2012 11:39:50 AM', '9/03/2012 11:39:50 AM', False, 'D4A462AB-DA14-46D2-9118-A00F00C037C6')";
        [Test]
        public void TestMethod()
        {
            // Arrange
            var engine = new FormattingEngine();
            var sut = new ParamBuilderFormatter(engine);

            // Act
            string result = sut.Execute(sample);


            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}