using System;
using System.Collections.Generic;
using Laan.Sql.Parser.Entities;
using System.Text;

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

        public virtual bool CanInline
        {
            get { return false; }
        }

    }
}
