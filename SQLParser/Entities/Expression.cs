using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Laan.SQL.Parser
{
    [DebuggerDisplay("{Value}")]
    public class Expression
    {
        public virtual string Value { get; protected set; }
    }

    public class OperatorExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public string Operator { get; set; }

        public override string Value
        {
            get { return String.Format( "{0} {1} {2}", Left.Value, Operator, Right.Value ); }
        }
    }

    public class SelectExpression : Expression
    {

        public SelectExpression()
        {
            Statement = new SelectStatement();
        }

        public SelectStatement Statement { get; set; }

        public override string Value
        {
            get { return Statement.ToString(); }
        }
    }

    public class FunctionExpression : Expression
    {
        public FunctionExpression()
        {
            Arguments = new List<Expression>();
        }

        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }

        public override string Value
        {
            get
            {
                string[] args = Arguments.Select( arg => arg.Value ).ToArray();
                return String.Format( "{0}({1})", Name, String.Join( Constants.COMMA, args ) );
            }
        }
    }

    public class StringExpression : Expression
    {
        public StringExpression( string value )
        {
            Value = value;
        }

        public override string Value
        {
            get { return Constants.QUOTE + base.Value + Constants.QUOTE; }
        }
    }
    
    public class IdentifierExpression : Expression
    {
        public IdentifierExpression( string value )
        {
            Parts = value.Split( new string[] { Constants.DOT }, StringSplitOptions.RemoveEmptyEntries ).ToList();
        }

        public List<string> Parts { get; set; }

        public override string Value
        {
            get { return String.Join( Constants.DOT, Parts.ToArray() ); }
        }
    }
    
    public class NestedExpression : Expression
    {
        public Expression Expression { get; set; }

        public override string Value
        {
            get { return Constants.OPEN_BRACKET + Expression.Value + Constants.CLOSE_BRACKET; }
        }
    }
}