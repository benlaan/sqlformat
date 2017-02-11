using System;
using System.Linq;

using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SelectCursorLeftWordAction : BaseLeftCusorAction
    {
        public SelectCursorLeftWordAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSelectCursorLeftWordAction";
            DisplayName = "Select Word To Left";
            DescriptivePhrase = "Selects one word to the Left";

            ButtonText = DisplayName;
            ToolTip = DisplayName;
            KeyboardBinding = "Text Editor::Shift+Alt+Left Arrow";
            ImageIndex = 5;
        }

        public override void Execute()
        {
            CursorLeft(true);
        }
    }
}