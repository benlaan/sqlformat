using System;
using System.Linq;
using System.Text;

using Laan.Sql.Formatter;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class CustomExpressionFormatter<T> : IIndentable, IExpressionFormatter where T : Expression
    {
        protected T _expression;
        private IIndentable _parent;
        private FormattingOptions _options;

        public CustomExpressionFormatter(T expression) : this(expression, null)
        {
        }

        public CustomExpressionFormatter(T expression, IIndentable parent)
        {
            _expression = expression;
            _parent = parent;
            _options = parent?.Options ?? new FormattingOptions();
            Indent = _options.UseSpaces ? new string(' ', _options.IndentSize) : "\t";
        }

        protected bool CanInlineExpression(Expression expr, int offset)
        {
            return expr is IInlineFormattable
                && ((IInlineFormattable)expr).CanInline
                && expr.Value.Length < _options.MaxLineLength - offset;
        }

        protected int GetCurrentColumn(StringBuilder sql)
        {
            return sql.ToString().Split('\n').Last().Length;
        }

        protected string GetIndent(bool includeNewLine)
        {
            var result = new StringBuilder(includeNewLine ? Environment.NewLine : String.Empty);

            for (int index = 0; index < IndentLevel; index++)
                result.Append(Indent);

            return result.ToString();
        }

        protected static string GetSpaces(int offset)
        {
            return new string(' ', offset);
        }

        protected string Keyword(string keyword)
        {
            return KeywordTransform.Apply(keyword, Options.KeywordCasing);
        }

        protected T _statement;


        public virtual string Execute()
        {
            return _expression.Value;
        }

        public int Offset { get; set; }

        public string Indent { get; set; }
        public int IndentLevel { get; set; }

        public void IncreaseIndent()
        {
            IndentLevel++;
        }

        public void DecreaseIndent()
        {
            IndentLevel--;
        }

        public FormattingOptions Options
        {
            get { return _options; }
        }
    }
}
