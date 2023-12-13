using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public interface IIndentable
    {
        void IncreaseIndent();
        void DecreaseIndent();
        string Indent { get; set; }
        int IndentLevel { get; set; }
    }
}
