using System;
using System.Collections.Generic;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class StatementFormatterFactory
    {
        private static Dictionary<Type, Type> _formatters;

        static StatementFormatterFactory()
        {
            _formatters = new Dictionary<Type, Type>
            {
                { typeof( SelectStatement ), typeof( SelectStatementFormatter ) },
                { typeof( UpdateStatement ), typeof( UpdateStatementFormatter ) },
                { typeof( DeleteStatement ), typeof( DeleteStatementFormatter ) },
                { typeof( InsertStatement ), typeof( InsertStatementFormatter ) },
                { typeof( DeclareStatement ), typeof( DeclareStatementFormatter ) },
                { typeof( GoTerminator ), typeof( GoTerminatorFormatter ) },
                { typeof( IfStatement ), typeof( IfStatementFormatter ) },
                { typeof( BeginTransactionStatement ), typeof( BeginTransactionStatementFormatter ) },
                { typeof( RollbackTransactionStatement ), typeof( RollbackTransactionStatementFormatter ) },
                { typeof( CommitTransactionStatement ), typeof( CommitTransactionStatementFormatter ) },
                { typeof( BlockStatement ), typeof( BlockStatementFormatter ) }
            };
        }

        public static IStatementFormatter GetFormatter( IIndentable indentable, StringBuilder outSql, IStatement statement )
        {
            Type formatterType;
            if ( !_formatters.TryGetValue( statement.GetType(), out formatterType ) )
                throw new FormatterNotImplementedException(
                    "Formatter not implemented for statement: " + statement.GetType().Name
                );

            var formatter = Activator.CreateInstance(
                formatterType,
                indentable,
                outSql,
                statement
            ) as IStatementFormatter;

            if ( formatter == null )
                throw new ArgumentNullException( "Formatter not instantiated: " + formatterType.Name );

            return formatter;
        }
    }
}
