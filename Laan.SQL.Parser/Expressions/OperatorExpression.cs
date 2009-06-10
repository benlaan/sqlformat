using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.SQL.Parser.Expressions
{
    public class OperatorExpression : Expression, IInlineFormattable
    {
        /// <summary>
        /// Initializes a new instance of the OperatorExpression class.
        /// </summary>
        public OperatorExpression( Expression parent ) : base ( parent )
        {
        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public string Operator { get; set; }

        public override string Value
        {
            get { return String.Format( "{0} {1} {2}", Left.Value, Operator, Right.Value ); }
        }

        #region IInlineFormattable Members

        public bool CanInline
        {
            get { return true; }
        }

        #endregion
    }
}
