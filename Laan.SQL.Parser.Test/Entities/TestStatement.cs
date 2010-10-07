using MbUnit.Framework;
using Laan.Sql.Parser.Entities;

namespace Laan.SQLParser.Test.Entities
{
    [TestFixture]
    public class TestStatement
    {
        [Test]
        public void Get_Identifier()
        {
            var sut = new Statement();

            var result = sut.Identifier;

            Assert.AreEqual( string.Empty, result );
        }
    }

}
