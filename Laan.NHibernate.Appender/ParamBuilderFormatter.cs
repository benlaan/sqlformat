using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Formatter;
using Laan.Sql.Parser;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Laan.NHibernate.Appender
{
    public class ParamBuilderFormatter
    {
        private readonly IFormattingEngine _engine;

        /// <summary>
        /// Initializes a new instance of the ParamBuilderFormatter class.
        /// </summary>
        /// <param name="engine"></param>
        public ParamBuilderFormatter(IFormattingEngine engine)
        {
            _engine = engine;
        }

        public string Execute(string sql)
        {
            var originalSql = sql;
            // designed to convert the following format
            // "SELECT * FROM Table WHERE ID=@P1 AND Name=@P2;@P1=20,@P2='Users'"
            try
            {
                // clean NHibernate rubbish from front of SQL statement
                const string batchCommands = "Batch commands:\r\n";
                if (sql.StartsWith(batchCommands))
                {
                    sql = sql.Remove(0, batchCommands.Length);

                    const string command = "command";

                    if (sql.StartsWith(command))
                    {
                        int colonPos = sql.IndexOf(":", StringComparison.Ordinal);

                        if (colonPos > 0)
                        {
                            sql = sql.Remove(0, colonPos+1);
                        }
                    }
                }

                var parameterSubstituter = new ParameterSubstituter();
                return _engine.Execute(parameterSubstituter.UpdateParamsWithValues(sql));
            }
            catch (Exception ex)
            {
                return String.Format("-- Error: {0}\n{1}", ex.Message, originalSql);
            }
        }
    }
}
