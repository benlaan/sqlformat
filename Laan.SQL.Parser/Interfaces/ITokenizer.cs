namespace Laan.Sql.Parser
{
    public interface ITokenizer
    {
        bool HasMoreTokens { get; }

        bool IsNextToken( params string[] tokenSet );

        bool TokenEquals( string value );

        void ReadNextToken();

        Token Current { get; }

        /// <summary>
        /// Verify current token matches expected token. Read next token if successful.
        /// </summary>
        /// <param name="token">Expected token</param>
        /// <exception cref="ExpectedTokenNotFoundException">current token did not match</exception>
        void ExpectToken( string token );

        void ExpectTokens( string[] tokens );

        Position Position { get; }

        BracketStructure ExpectBrackets();
    }
}
