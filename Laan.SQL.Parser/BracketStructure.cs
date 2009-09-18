using System;

namespace Laan.SQL.Parser
{
    public class BracketStructure : IDisposable
    {
        private CustomTokenizer _tokenizer;

        public BracketStructure( CustomTokenizer tokenizer )
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
