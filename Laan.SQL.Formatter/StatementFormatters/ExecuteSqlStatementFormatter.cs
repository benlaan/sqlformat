using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Expressions;
using System.Collections.Generic;
using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class ExecuteSqlStatementFormatter : StatementFormatter<ExecuteSqlStatement>, IStatementFormatter
    {
        public ExecuteSqlStatementFormatter(IIndentable indentable, StringBuilder sql, ExecuteSqlStatement statement)
            : base(indentable, sql, statement)
        {
            
        }
        
        #region IStatementFormatter Members

        public void Execute()
        {
            var statements = new List<IStatement>();

            if (_statement.Arguments.Any())
            {
                DeclareStatement declare = new DeclareStatement();
                declare.Definitions.AddRange(
                    _statement.Arguments.Select(a => new VariableDefinition(a.Name, a.Type))
                );
                statements.Add(declare);

                SelectStatement initialise = new SelectStatement();
                initialise.Fields.AddRange(
                    _statement.Arguments.Select(a =>
                        new Field
                        {
                            Expression = new CriteriaExpression(null) 
                            { 
                                Left = new StringExpression(a.Name, null), 
                                Operator = Constants.Assignment,
                                Right= new StringExpression(a.Value.ToString(), null)
                            }
                        }
                    )
                );
                statements.Add(initialise);
            }
            statements.Add(_statement.InnerStatement);

            foreach ( IStatement statement in statements )
            {
                FormatStatement( statement );
                if (statement != statements.Last())
                    NewLine( 2 );
            }
        }

        #endregion
    }
}
