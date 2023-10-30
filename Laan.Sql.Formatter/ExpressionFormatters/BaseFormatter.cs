using System;
using System.Collections.Generic;
using System.Text;

namespace Laan.Sql.Formatter
{
    public class BaseFormatter : IIndentable
    {
        private static BracketFormatOption _bracketSpaceOption = BracketFormatOption.NoSpaces;
        protected static Dictionary<BracketFormatOption, string> _bracketFormats;

        protected StringBuilder _sql;
        protected IIndentable _indentable;

        public BaseFormatter(IIndentable indentable, StringBuilder sql)
        {
            _sql = sql;
            _indentable = indentable;
        }

        public static string FormatBrackets(string text)
        {
            return String.Format(_bracketFormats[_bracketSpaceOption], text);
        }

        protected void AppendFormat(string text, params object[] args)
        {
            Append(String.Format(text, args));
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

        protected void NewLine(int times)
        {
            for (int index = 0; index < times; index++)
                _sql.AppendLine();
        }

        protected void NewLine()
        {
            NewLine(1);
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

        public virtual bool CanInline
        {
            get { return false; }
        }
    }
}
