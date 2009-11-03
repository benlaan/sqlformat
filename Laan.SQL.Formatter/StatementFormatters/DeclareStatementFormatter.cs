using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class DeclareStatementFormatter : CustomStatementFormatter<DeclareStatement>, IStatementFormatter
    {
        public DeclareStatementFormatter( IIndentable indentable, StringBuilder sql, DeclareStatement statement )
            : base( indentable, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            IndentAppend( "DECLARE" );

            var maxNameLength = _statement.Definitions.Max( def => def.Name.Length ) * -1;
            var maxTypeLength = _statement.Definitions.Max( def => def.Type.Length ) * -1;
            string format = String.Format( "{{0,{0}}} {{1,{1}}}{{2}}", maxNameLength, maxTypeLength );

            int count = _statement.Definitions.Count;
            using ( new IndentScope( this ) )
            {
                foreach ( var def in _statement.Definitions )
                {
                    NewLine();
                    var variableDecaration = String.Format(
                        format,
                        def.Name,
                        def.Type,
                        ( def.DefaultValue != null ? " = " + def.DefaultValue.FormattedValue( 0, this ) : "" )
                    ).TrimEnd() + ( --count > 0 ? "," : "" );

                    IndentAppend( variableDecaration );
                }
            }
        }

        #endregion
    }
}
