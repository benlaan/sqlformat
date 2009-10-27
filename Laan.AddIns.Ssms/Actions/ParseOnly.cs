using System;

using Laan.AddIns.Core;
using Laan.SQL.Parser;

namespace Laan.AddIns.Ssms.Actions
{
    public abstract class ParseOnly : Laan.AddIns.Core.Action
    {
        public ParseOnly( AddIn addIn ) : base( addIn )
        {

        }

        protected abstract string ProcessToken( string value );

        public override bool CanExecute()
        {
            return true; // !String.IsNullOrEmpty( _addIn.TextDocument.Selection.Text );
        }

        public override void Execute()
        {
            var all = _addIn.AllText;
            var sql = _addIn.TextDocument.Selection.Text;
            var tokenizer = new SqlTokenizer( sql );
            tokenizer.SkipWhiteSpace = false;

            var output = new System.Text.StringBuilder();

            tokenizer.ReadNextToken();
            while ( tokenizer.HasMoreTokens )
            {
                if ( tokenizer.Current != (Token)null )
                    output.Append( ProcessToken( tokenizer.Current.Value ) );

                tokenizer.ReadNextToken();
            }
            _addIn.InsertText( output.ToString() );
        }
    }
}
