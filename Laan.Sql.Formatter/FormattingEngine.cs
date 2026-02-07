using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class FormattingEngine : IFormattingEngine
    {
        public FormattingEngine() : this(new FormattingOptions())
        {
        }

        public FormattingEngine(FormattingOptions options)
        {
            Options = options ?? new FormattingOptions();
            Options.Validate();
        }

        /// <summary>
        /// Creates a FormattingEngine with options loaded from a config file
        /// </summary>
        public static FormattingEngine CreateWithConfigFile(string configPath = null)
        {
            var options = configPath != null
                ? FormattingOptionsLoader.LoadFromFile(configPath)
                : FormattingOptionsLoader.TryLoadFromHierarchy();
            
            return new FormattingEngine(options);
        }

        public string Execute(string sql)
        {
            var outSql = new StringBuilder(sql.Length * 2);
            var statements = ParserFactory.Execute(sql);

            var indentation = new Indentation(Options);

            foreach (var statement in statements)
            {
                var formatter = StatementFormatterFactory.GetFormatter(indentation, outSql, statement);
                formatter.Execute();

                if (statement != statements.Last())
                    outSql.AppendLine(Environment.NewLine);
            }

            return outSql.ToString();
        }

        public FormattingOptions Options { get; private set; }

        // Legacy properties for backward compatibility
        public int TabSize 
        { 
            get { return Options.IndentSize; }
            set { Options.IndentSize = value; }
        }
        
        public bool UseTabChar 
        { 
            get { return !Options.UseSpaces; }
            set { Options.UseSpaces = !value; }
        }
    }
}
