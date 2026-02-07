using System;
using System.Collections.Generic;
using System.Text;

namespace Laan.Sql.Formatter
{
    public class BaseFormatter : IIndentable
    {
        protected static Dictionary<BracketFormatOption, string> _bracketFormats;

        protected StringBuilder _sql;
        protected IIndentable _indentable;

        public BaseFormatter(IIndentable indentable, StringBuilder sql)
        {
            _sql = sql;
            _indentable = indentable;
        }

        protected string FormatBrackets(string text)
        {
            var bracketOption = Options.BracketSpacing == BracketSpacing.WithSpaces 
                ? BracketFormatOption.SpacesWithinBrackets 
                : BracketFormatOption.NoSpaces;
            return String.Format(_bracketFormats[bracketOption], text);
        }

        // Keep static version for backward compatibility
        public static string FormatBrackets(string text, BracketSpacing spacing)
        {
            var bracketOption = spacing == BracketSpacing.WithSpaces 
                ? BracketFormatOption.SpacesWithinBrackets 
                : BracketFormatOption.NoSpaces;
            return String.Format(_bracketFormats[bracketOption], text);
        }

        protected void IndentAppend(string text)
        {
            for (int count = 0; count < IndentLevel; count++)
                _sql.Append(Indent);
            _sql.Append(text);
        }

        protected void IndentAppendFormat(string text, params object[] args)
        {
            IndentAppend(String.Format(text, args));
        }

        protected void IndentAppendKeyword(string keyword)
        {
            IndentAppend(Keyword(keyword));
        }

        protected void IndentAppendLine(string text)
        {
            IndentAppend(text);
            NewLine();
        }

        protected void IndentAppendLineFormat(string text, params object[] args)
        {
            IndentAppendLine(String.Format(text, args));
        }

        protected void Append(string text)
        {
            _sql.Append(text);
        }

        protected void AppendKeyword(string keyword)
        {
            _sql.Append(Keyword(keyword));
        }

        protected string Keyword(string keyword)
        {
            return KeywordTransform.Apply(keyword, Options.KeywordCasing);
        }

        protected void NewLine(int times = 1)
        {
            for (int index = 0; index < times; index++)
                _sql.AppendLine();
        }

        public string Indent
        {
            get { return _indentable.Indent; }
            set { _indentable.Indent = value; }
        }

        public int IndentLevel
        {
            get { return _indentable.IndentLevel; }
            set { _indentable.IndentLevel = value; }
        }

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
            get { return _indentable.Options; }
        }

        public virtual bool CanInline
        {
            get { return false; }
        }
    }
}
