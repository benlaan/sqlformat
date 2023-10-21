using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.Sql.Parser.Expressions
{
    public class ExpressionList : Expression, IInlineFormattable
    {
        public ExpressionList() : base( null )
        {
            Identifiers = new List<Expression>();
        }

        public List<Expression> Identifiers { get; set; }

        public override string Value
        {
            get { return String.Join( ", ", Identifiers.Select( id => id.Value ).ToArray() ); }
        }

        #region IInlineFormattable Members

        public override bool CanInline
        {
            get { return Value.Length < 80 && Identifiers.All( id => id.CanInline ); }
        }

        #endregion
    }
}
