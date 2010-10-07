using MbUnit.Framework;
using Laan.Sql.Parser.Entities;

namespace Laan.SQLParser.Test.Entities
{
    [TestFixture]
    public class TestAlias
    {

        [Test]
        [Row("test name")]
        public void Implicit_Value( string name )
        {
            // Arrange
            var sut = new Alias( null );
            sut.Name = name;
            sut.Type = AliasType.Implicit;

            // Act
            var result = sut.Value;

            // Assert
            Assert.AreEqual( " " + name, result );
        }

        [Test]
        [Row("test name")]
        public void Equals_Value( string name )
        {
            // Arrange
            var sut = new Alias( null );
            sut.Name = name;
            sut.Type = AliasType.Equals;

            // Act
            var result = sut.Value;

            // Assert
            Assert.AreEqual( name + " = ", result );
        }

        [Test]
        [Row("test name")]
        public void As_Value( string name )
        {
            // Arrange
            var sut = new Alias( null );
            sut.Name = name;
            sut.Type = AliasType.As;

            // Act
            var result = sut.Value;

            // Assert
            Assert.AreEqual( " AS " + name, result );
        }
    }
}
