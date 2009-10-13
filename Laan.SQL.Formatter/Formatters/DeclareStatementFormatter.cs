using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class DeclareStatementFormatter : CustomFormatter<DeclareStatement>, IStatementFormatter
    {
        public DeclareStatementFormatter( string indent, int indentStep, StringBuilder sql, DeclareStatement statement )
            : base( indent, indentStep, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            Append( "DECLARE" );

            var maxNameLength = _statement.Definitions.Max( def => def.Name.Length ) * -1;
            var maxTypeLength = _statement.Definitions.Max( def => def.Type.Length ) * -1;

            string format = String.Format( "{{0,{0}}} {{1,{1}}}{{2}}", maxNameLength, maxTypeLength );

            int count = _statement.Definitions.Count;
            _indentStep++;
            foreach ( var def in _statement.Definitions )
            {
                NewLine();
                var variableDecaration = String.Format(
                    format,
                    def.Name,
                    def.Type,
                    ( def.DefaultValue != null ? " = " + def.DefaultValue.FormattedValue( 0, _indent, _indentStep + 1 ) : "" )
                ).TrimEnd() + ( --count > 0 ? "," : "" );

                Append( variableDecaration );
            }
            _indentStep--;
        }

        #endregion
    }
}
