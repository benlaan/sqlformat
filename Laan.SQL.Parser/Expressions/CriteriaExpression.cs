using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Laan.SQL.Parser.Expressions
{
    public interface IInlineFormattable
    {
        bool CanInline { get; }
    }

    public class CriteriaExpression : Expression, IInlineFormattable
    {
        private string _operator;

        public CriteriaExpression( Expression parent ) : base( parent )
        {
            Left = new Expression( this );
            Right = new Expression( this );

        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
        
        public string Operator
        {
            get { return _operator; }
            set { _operator = value.ToUpper(); }
        }

        public override string Value
        {
            get { return String.Format( "{0} {1} {2}", Left.Value, Operator, Right.Value ); }
        }

        #region IInlineFormattable Members

        public bool CanInline
        {
            get { return false; }
        }

        #endregion
    }
}
