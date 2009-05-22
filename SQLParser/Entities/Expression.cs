using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
                return String.Format( "{0}({1})", Name, String.Join( Constants.Comma, args ) );
            }
        }
    }

    public class StringExpression : Expression
    {
        public StringExpression( string value )
        {
            Value = value;
        }
    }
    
    public class IdentifierExpression : Expression
    {
        public IdentifierExpression( string value )
        {
            Parts = value.Split( new string[] { Constants.Dot }, StringSplitOptions.RemoveEmptyEntries ).ToList();
        }

        public List<string> Parts { get; set; }

        public override string Value
        {
            get { return String.Join( Constants.Dot, Parts.ToArray() ); }
        }
    }
    
    public class NestedExpression : Expression
    {
        public Expression Expression { get; set; }

        public override string Value
        {
            get { return Constants.OpenBracket + Expression.Value + Constants.CloseBracket; }
        }
    }

    public class CaseSwitch
    {
        public Expression When { get; set; }
        public Expression Then { get; set; }

        public CaseSwitch()
        {
            When = new Expression();
            Then = new Expression();
        }
    }

    public abstract class CaseExpression : Expression
    {
        public List<CaseSwitch> Cases { get; set; }
        public Expression Else { get; set; }

        protected string GetElseToString()
        {
            return Else != null ? " ELSE " + Else.Value : "";
        }
        
        protected string GetCasesToString()
        {
            StringBuilder cases = new StringBuilder();
            foreach ( var caseSwitch in Cases )
                cases.AppendFormat(
                        " WHEN {0} THEN {1}",
                        caseSwitch.When.Value, caseSwitch.Then.Value
                    );

            return cases.ToString();
        }

        public CaseExpression()
        {
            Cases = new List<CaseSwitch>();
        }
    }

    public class CaseSwitchExpression : CaseExpression
    {
        public Expression Switch { get; set; }

        public CaseSwitchExpression() : base() { }

        public override string Value
        {
            get
            {
                return String.Format(
                    "CASE {0}{1}{2} END",
                    Switch.Value, GetCasesToString(), GetElseToString() 
                );
            }
        }
    }

    public class CaseWhenExpression : CaseExpression
    {
        public CaseWhenExpression() : base() { }

        public override string Value
        {
            get
            {
                return String.Format(
                    "CASE{0}{1} END", GetCasesToString(), GetElseToString()
                );
            }
        }
    }
}