using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.SQLParser.Test.Entities
{
    [TestFixture]
    public class TestAlias
    {

        [Test]
        [TestCase("test name")]
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
        [TestCase("test name")]
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
        [TestCase("test name")]
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
