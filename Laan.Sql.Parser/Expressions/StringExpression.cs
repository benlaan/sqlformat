using System;
namespace Laan.Sql.Parser.Expressions
{
    public class StringExpression : Expression
    {
        public StringExpression( string value, Expression parent ) : base ( parent )
        {
            Value = value;
        }

        public override bool CanInline
        {
            get { return true; }
        }

        public string Content
        {
            get
            {
                string content = Value.Replace("''", "'");
                if (content.StartsWith("N'"))
                    content = content.Substring(1);
                return content.Trim(new[] { '\'' });
            }
        }
    }
}
