using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class VariableDefinitionFormatter : BaseFormatter
    {
        private IList<VariableDefinition> _definitions;

        public VariableDefinitionFormatter(IList<VariableDefinition> definitions, IIndentable indentable, StringBuilder sql) : base(indentable, sql)
        {
            _definitions = definitions;
        }

        public void Execute()
        {
            var maxNameLength = _definitions.Max(def => def.Name.Length) * -1;
            var maxTypeLength = _definitions.Max(def => def.Type.Length) * -1;
            string format = String.Format("{{0,{0}}} {{1,{1}}}{{2}}", maxNameLength, maxTypeLength);

            int count = _definitions.Count;

            using (new IndentScope(this))
            {
                foreach (var definition in _definitions)
                {
                    NewLine();

                    var variableDecaration = String.Format(
                        format,
                        definition.Name,
                        definition.Type,
                        (definition.DefaultValue != null ? " = " + definition.DefaultValue.FormattedValue(0, this) : "")
                    ).TrimEnd() + (--count > 0 ? "," : "");

                    IndentAppend(variableDecaration);
                }
            }
        }
    }
}
