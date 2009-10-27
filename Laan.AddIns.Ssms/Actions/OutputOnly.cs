using System;

using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class OutputOnly : ParseOnly
    {
        public OutputOnly( AddIn addIn ) : base( addIn )
        {
            KeyName = "LaanSqlPareseOnly";
            DisplayName = "Output Parsed SQL";
            DescriptivePhrase = "Outputting Parsed SQL";

            ButtonText = "&Parse SQL";
            ToolTip = "parses and outputs SQL";
            ImageIndex = 59;
            KeyboardBinding = "Text Editor::Ctrl+Shift+E";
        }

        protected override string ProcessToken( string value )
        {
            return value;
        }
    }
}
