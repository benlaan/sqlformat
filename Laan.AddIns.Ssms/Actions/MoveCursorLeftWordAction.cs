using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class MoveCursorLeftWordAction : BaseLeftCusorAction
    {
        public MoveCursorLeftWordAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlMoveCursorLeftWordAction";
            DisplayName = "Move Cursor Left One Word";
            DescriptivePhrase = "Moves the cursor one word to the Left";

            ButtonText = DisplayName;
            ToolTip = DisplayName;
            KeyboardBinding = "Text Editor::Alt+Left Arrow";
            ImageIndex = 5;
        }

        public override void Execute()
        {
            CursorLeft(false);
        }

        public override bool CanExecute()
        {
            return base.CanExecute() 
                && AddIn.CurrentSelection.Length == 0;
        }
    }
}