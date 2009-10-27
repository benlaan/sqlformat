using System;
using System.Text;
using System.Linq;
using Laan.SQL.Parser;
using System.Collections.Generic;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Formatter
{
    public interface IIndentable
    {
        void IncreaseIndent();
        void DecreaseIndent();
        string Indent { get; set; }
        int IndentLevel { get; set; }
    }
}
