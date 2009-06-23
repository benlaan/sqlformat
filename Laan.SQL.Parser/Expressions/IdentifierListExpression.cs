using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.SQL.Parser.Expressions
{
    public class IdentifierListExpression : Expression, IInlineFormattable
    {
        public IdentifierListExpression() : base( null )
        {
            Identifiers = new List<IdentifierExpression>();
        }

        public List<IdentifierExpression> Identifiers { get; set; }

        public override string Value
        {
            get { return String.Join( ", ", Identifiers.Select( id => id.Value ).ToArray() ); }
        }

        #region IInlineFormattable Members

        public bool CanInline
        {
            get { return true; }
        }

        #endregion
    }
}
