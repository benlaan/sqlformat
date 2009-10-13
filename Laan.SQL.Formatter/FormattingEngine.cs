using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class FormattingEngine : IFormattingEngine
    {
        private static Dictionary<Type, Type> _formatters;

        static FormattingEngine()
        {
            _formatters = new Dictionary<Type, Type>
            {
                { typeof( SelectStatement ), typeof( SelectStatementFormatter ) },
                { typeof( UpdateStatement ), typeof( UpdateStatementFormatter ) },
                { typeof( DeleteStatement ), typeof( DeleteStatementFormatter ) },
                { typeof( InsertStatement ), typeof( InsertStatementFormatter ) },
                { typeof( DeclareStatement ), typeof( DeclareStatementFormatter ) },
                { typeof( GoTerminator ), typeof( GoTerminatorFormatter ) },
//                { typeof( CreateTableStatement ), typeof( CreateTableStatementFormatter ) }
            };
        }

        /// <summary>
        /// Initializes a new instance of the FormattingEngine class.
        /// </summary>
        public FormattingEngine()
        {
            IndentStep = 0;
            TabSize = 4;
            UseTabChar = false;
        }

        private IStatementFormatter GetFormatter( string indent, StringBuilder outSql, IStatement statement )
        {
            Type formatterType;
            if ( !_formatters.TryGetValue( statement.GetType(), out formatterType ) )
                throw new FormatterNotImplementedException(
                    "Formatter not implemented for statement: " + statement.GetType().Name 
                );

            var formatter = Activator.CreateInstance(
                formatterType,
                indent,
                IndentStep,
                outSql,
                statement
            ) as IStatementFormatter;

            if ( formatter == null )
                throw new ArgumentNullException( "Formatter not instantiated: " + formatterType.Name );
            
            return formatter;
        }

        public string Execute( string sql )
        {
            string indent = UseTabChar ? "\t" : new string( ' ', TabSize );

            var outSql = new StringBuilder();
            var statements = ParserFactory.Execute( sql );

            foreach ( var statement in statements )
            {
                var formatter = GetFormatter( indent, outSql, statement );
                formatter.Execute();

                if ( statement != statements.Last() )
                    outSql.AppendLine( "\n" );
            }
            return outSql.ToString();
        }

        public int IndentStep { get; set; }
        public int TabSize { get; set; }
        public bool UseTabChar { get; set; }
    }
}
