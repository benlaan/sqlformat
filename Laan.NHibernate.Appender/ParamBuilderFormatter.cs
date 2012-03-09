using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Formatter;
using Laan.Sql.Parser;

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

                sql = ParserFactory.TrimBatchMetadata(sql);

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
