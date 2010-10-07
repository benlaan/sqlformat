using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Laan.Sql.Parser.Exceptions;

namespace Laan.SQLParser.Test.Exceptions
{
    [TestFixture]
    public class TestExceptions
    {
        [VerifyContract]
        public readonly IContract ExpectedTokenNotFoundExceptionTests = new ExceptionContract<ExpectedTokenNotFoundException>()
        {
            ImplementsSerialization = false, // Optional (default is true)
            ImplementsStandardConstructors = true // Optional (default is true)
        };

        [VerifyContract]
        public readonly IContract SyntaxExceptionTests = new ExceptionContract<SyntaxException>()
        {
            ImplementsSerialization = false, // Optional (default is true)
            ImplementsStandardConstructors = true // Optional (default is true)
        };

        [VerifyContract]
        public readonly IContract UnknownTokenExceptionTests = new CustomExceptionContract<UnknownTokenException>()
        {
            ImplementsSerialization = false, // Optional (default is true)
            ImplementsStandardConstructors = true // Optional (default is true)
        };

        [VerifyContract]
        public readonly IContract ParserNotImplementedExceptionTests = new ExceptionContract<ParserNotImplementedException>()
        {
            ImplementsSerialization = false, // Optional (default is true)
            ImplementsStandardConstructors = true // Optional (default is true)
        };

        [VerifyContract]
        public readonly IContract FormatterNotImplementedExceptionTests = new ExceptionContract<FormatterNotImplementedException>()
        {
            ImplementsSerialization = false, // Optional (default is true)
            ImplementsStandardConstructors = true // Optional (default is true)
        };
    }

}
