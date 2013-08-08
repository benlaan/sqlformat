using System.Linq;
using System.Collections.Generic;
using System;

using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestNHibernateStatements
    {
        private const string sample =
    @"Batch commands:
command 0:INSERT INTO dbo.[SimpleEntity] (Version, Name, UserName, Created, Modified, IsDeleted, Id) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";

        [Test]
        public void Test_ParserFactory_Correctly_Strips_Batch_Commands_Preamble()
        {
            // Arrange

            // Act
            List<IStatement> results = ParserFactory.Execute(sample);


            // Assert
            Assert.IsNotEmpty(results);
            Assert.IsInstanceOf<InsertStatement>(results.First());
        }

    }
}