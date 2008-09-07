using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SQLParser
{
    public class TokenizerRule
    {
        public Func<int, bool> StartOp { get; set; }
        public Func<int, bool> ContinueOp { get; set; }
    }

    public class Tokenizer
    {
        private Func<int, bool> _neverContinue = i => false;
        private TokenizerRule[] _tokenizingRules;
        private StringReader _reader;
        private string _currentToken;

        public Tokenizer( string sql )
        {
            _reader = new StringReader( sql );
            Initialize();
        }

        public Tokenizer( StringReader reader )
        {
            _reader = reader;
            Initialize();
        }

        private void Initialize()
        {
            _tokenizingRules = new TokenizerRule[] 
            { 
                new TokenizerRule { StartOp = IsAlpha,   ContinueOp = IsAlphaNumeric },
                new TokenizerRule { StartOp = IsNumeric, ContinueOp = IsNumeric }, 
                new TokenizerRule { StartOp = IsSpecialChar, ContinueOp = _neverContinue }, 
            };
        }

        #region Utilities

        private bool IsWithinSet( int readChar, char[] set )
        {
            return set.Contains( (char) readChar );
        }

        private bool IsBetween( int readChar, char fromChar, char toChar )
        {
            return readChar >= fromChar && readChar <= toChar;
        }

        private bool IsSpecialChar( int readChar )
        {
            return IsWithinSet( readChar, new char[] { '.', ',', '@', '/', '*', '^', '(', ')', '[', ']', '\'', '"' } );
        }

        private bool IsAlpha( int readChar )
        {
            return 
                IsBetween( readChar, 'A', 'Z' ) || 
                IsBetween( readChar, 'a', 'z' ) || 
                IsWithinSet( readChar, new char[] { '_' } );
        }

        private bool IsNumeric( int readChar )
        {
            return IsBetween( readChar, '0', '9' ) || readChar == '.';
        }

        private bool IsAlphaNumeric( int readChar )
        {
            return IsAlpha( readChar ) || IsNumeric( readChar );
        }

        #endregion

        private bool GetToken( Func<int, bool> startAction, Func<int, bool> continuingAction, int readChar, out string token )
        {
            token = "";
            if ( startAction( readChar ) )
            {
                StringBuilder tokenBuilder = new StringBuilder();
                do
                {
                    readChar = _reader.Read();
                    tokenBuilder.Append( (char) readChar );
                    readChar = _reader.Peek();
                }
                while ( readChar != -1 && continuingAction( readChar ) );

                token = tokenBuilder.ToString();
                return true;
            }
            return false;
        }

        public bool HasMoreTokens
        {
            get { return _reader.Peek() != -1; }
        }

        /// <summary>
        /// Utility (psuedo-function) to check that the current token equals the input parameter
        /// if so, the current token is advanced
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TokenEquals( string value )
        {
            bool areEqual = Current.ToLower() == value.ToLower();
            if ( areEqual )
                ReadNextToken();

            return areEqual;
        }
        
        /// <summary>
        /// Consumes the input stream until the next token is found
        /// </summary>
        public void ReadNextToken()
        {
            int readChar;
            string token = "";
            do
            {
                readChar = _reader.Peek();
                foreach ( var rule in _tokenizingRules )
                {
                    if ( GetToken( rule.StartOp, rule.ContinueOp, readChar, out token ) )
                    {
                        _currentToken = token;
                        return;
                    }
                }
                // at this point, all other chars need to be read (consumed) if not processed by any of the 
                // above rules
                readChar = _reader.Read();

            } while ( readChar != -1 );

            _currentToken = token;
        }

        public string Current
        {
            get { return _currentToken; }
        }
        
    }
}
