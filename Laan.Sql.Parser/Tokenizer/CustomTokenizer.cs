using System;
using System.Linq;

using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser
{
    public abstract class CustomTokenizer
    {
        //private abstract void InternalSetSkipWhiteSpace();

        //public bool SkipWhiteSpace
        //{
        //    set { InternalSetSkipWhiteSpace(); }
        //}

        public virtual bool IsNextToken(params string[] tokenSet)
        {
            foreach (var token in tokenSet)
                if (Current == token)
                    return true;

            return false;
        }

        public bool TokenEquals(string value)
        {
            bool areEqual = Current == value;

            if (areEqual)
                ReadNextToken();

            return areEqual;
        }

        public virtual void ReadNextToken()
        {
            // do nothing
        }

        public virtual Token Current
        {
            get { return new Token(); }
        }

        /// <summary>
        /// Verify current token matches expected token. Read next token if successful.
        /// </summary>
        /// <param name="token">Expected token</param>
        /// <exception cref="ExpectedTokenNotFoundException">current token did not match</exception>
        public void ExpectToken(string token)
        {
            if (Current == Token.Null || Current != token)
                throw new ExpectedTokenNotFoundException(token, Current != Token.Null ? Current.Value : "", Position);

            ReadNextToken();
        }

        /// <summary>
        /// Verify current tokens match expected tokens. Read next token if successful.
        /// </summary>
        /// <param name="tokens">Expected tokens</param>
        /// <exception cref="ExpectedTokenNotFoundException">current token did not match</exception>
        public void ExpectTokens(string[] tokens)
        {
            foreach (string token in tokens)
                ExpectToken(token);
        }

        public Position Position { get; protected set; }

        public BracketStructure ExpectBrackets()
        {
            return new BracketStructure(this as ITokenizer);
        }

        public virtual bool HasMoreTokens
        {
            get { return false; }
        }
    }
}
