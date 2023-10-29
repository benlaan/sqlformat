using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class Indentation : IIndentable
    {
        public Indentation(int indentSize = 4, bool useTabs = false)
        {
            IndentLevel = 0;
            Indent = useTabs ? "\t" : new string(' ', indentSize);
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
    }
}
