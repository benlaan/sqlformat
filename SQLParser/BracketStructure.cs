using System;

namespace Laan.SQL.Parser
{
    public class BracketStructure : IDisposable
    {
        private Tokenizer _tokenizer;

        public BracketStructure( Tokenizer tokenizer )
        {
            _tokenizer = tokenizer;
            _tokenizer.ExpectToken( Constants.OpenBracket );
        }

        #region IDisposable Members

        public void Dispose()
        {
            _tokenizer.ExpectToken( Constants.CloseBracket );
        }

        #endregion
    }
}
