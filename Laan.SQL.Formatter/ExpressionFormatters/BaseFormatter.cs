using System;
using System.Collections.Generic;

namespace Laan.Sql.Formatter
{
    public class BaseFormatter
    {
        private BracketFormatOption bracketSpaceOption = BracketFormatOption.NoSpaces;
        protected static Dictionary<BracketFormatOption, string> _bracketFormats;

        public BaseFormatter()
        {

        }
        
        protected string FormatBrackets( string text )
        {
            return String.Format( _bracketFormats[bracketSpaceOption], text );
        }
    }
}
