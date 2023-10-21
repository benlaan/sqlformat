using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class Indentation : IIndentable
    {
        public Indentation()
        {
            IndentLevel = 0;
            Indent = new string(' ', 4);
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
