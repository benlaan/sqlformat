using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

using NUnit.Framework;

using Enumerable = System.Linq.Enumerable;
using System.Linq;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestTableHints
    {
        [Test]
        public void Update_With_Tablock()
        {
            // Arrange
            const string sql = @"UPDATE Production.Product
WITH (TABLOCK)
SET ListPrice = ListPrice * 1.10
WHERE ProductNumber LIKE 'BK-%';
";
            // Act
            var statement = Enumerable.First(ParserFactory.Execute<UpdateStatement>(sql));

            // Assert
            Assert.IsNotNull(statement);
            Assert.IsNotEmpty(statement.TableHints);
            Assert.IsTrue(statement.TableHints.Count(th => th.Hint == "TABLOCK") == 1);
        }

        [Test]
        public void Insert_With_Xlock()
        {
            // Arrange
            const string sql =
                @"INSERT INTO Production.Location WITH (XLOCK)
(Name, CostRate, Availability)
VALUES ( 'Final Inventory', 15, 80);
";

            // Act
            var statement = Enumerable.First(ParserFactory.Execute<InsertStatement>(sql));

            // Assert
            Assert.IsNotNull(statement);
            Assert.IsNotEmpty(statement.TableHints);
            Assert.IsTrue(statement.TableHints.Where(x => x.Hint == "XLOCK").Count() == 1);

        }

        [Test]
        public void Select_With_ForceSeek()
        {
            const string sql = @"SELECT *
FROM Sales.SalesOrderHeader AS h
INNER JOIN Sales.SalesOrderDetail AS d WITH (FORCESEEK)
    ON h.SalesOrderID = d.SalesOrderID 
WHERE h.TotalDue > 100
AND (d.OrderQty > 5 OR d.LineTotal < 1000.00);";

            // Act
            var statement = Enumerable.First(ParserFactory.Execute<SelectStatement>(sql));

            // Assert
            Assert.IsNotNull(statement);
            Assert.IsNotEmpty(statement.From);
            Assert.IsNotEmpty(statement.From[0].Joins);
            Assert.IsTrue(statement.From[0].Joins[0].TableHints.Where(x => x.Hint == "FORCESEEK").Count() == 1);
        }

        [Test]
        public void Select_With_Hint_Without_With()
        {
            // Arrange
            const string sql = @"SELECT * FROM t (TABLOCK)";

            // Act
            var statement = ParserFactory.Execute<SelectStatement>(sql).First();

            // Assert
            Assert.IsTrue(statement.From[0].TableHints.Where(x => x.Hint == "TABLOCK").Count() == 1, "Should be one hint - TABLOCK");
        }

        [Test]
        public void Select_With_Multiple_Hints()
        {
            // Arrange
            const string sql = "SELECT * FROM t WITH (TABLOCK,UPDLOCK,HOLDLOCK)";

            // Act
            var statement = ParserFactory.Execute<SelectStatement>(sql).First();

            // Assert
            Assert.IsTrue(statement.From[0].TableHints.Count() == 3, "Should be three hints - TABLOCK, UPdLOCK, HOLDLOCK");
        }

        [Test]
        public void Select_Without_With_Clause_And_Multiple_Hints()
        {
            // Arrange
            const string sql = "SELECT * FROM T (TABLOCK,UPDLOCK,HOLDLOCK)";

            // Act
            var statement = ParserFactory.Execute<SelectStatement>(sql).First();

            // Assert
            Assert.IsTrue(statement.From[0].TableHints.Count() == 3, "Should be three hints - TABLOCK, UPDLOCK, HOLDLOCK");
        }
    }
}