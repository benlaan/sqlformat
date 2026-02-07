using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class Indentation : IIndentable
    {
        public Indentation() : this(new FormattingOptions())
        {
        }

        public Indentation(FormattingOptions options)
        {
            Options = options ?? new FormattingOptions();
            IndentLevel = 0;
            
            // Build indent string based on options
            if (options.UseSpaces)
                Indent = new string(' ', options.IndentSize);
            else
                Indent = "\t";
        }

        public void IncreaseIndent()
        {
            IndentLevel++;
        }
        
        public void DecreaseIndent()
        {
            IndentLevel--;
        }

        public string Indent { get; set; }
        public int IndentLevel { get; set; }
        public FormattingOptions Options { get; private set; }
    }
}
