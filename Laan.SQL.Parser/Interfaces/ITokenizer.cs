using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser
{
    public interface ITokenizer
    {
        bool SkipWhiteSpace { set; }
        bool HasMoreTokens { get; }

        /// <summary>
        /// Returns true if <see cref="Current"/> is contained in <paramref name="tokenSet"/>
        /// </summary>
        /// <param name="tokenSet"></param>
        /// <returns></returns>
        bool IsNextToken( params string[] tokenSet );

        /// <summary>
        /// Check that the current token equals one of the supplied values.
        /// If so the current token is advanced
        /// </summary>
        /// <param name="value">Tokens to compare</param>
        /// <returns></returns>
        bool TokenEquals( string value );

        void ReadNextToken();

        Token Current { get; }

        /// <summary>
        /// Verify current token matches expected token. Read next token if successful.
        /// </summary>
        /// <param name="token">Expected token</param>
        /// <exception cref="ExpectedTokenNotFoundException">current token did not match</exception>
        void ExpectToken( string token );

        /// <summary>
        /// Verify current tokens matche expected tokens. Read next token if successful.
        /// </summary>
        /// <param name="tokens">Expected tokens</param>
        /// <exception cref="ExpectedTokenNotFoundException">current token did not match</exception>
        void ExpectTokens( string[] tokens );

        Position Position { get; }

        BracketStructure ExpectBrackets();
    }
}
