using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class MoveCursorRightWordAction : BaseRightCusorAction
    {
        public MoveCursorRightWordAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlMoveCursorRightWordAction";
            DisplayName = "Move Cursor Right One Word";
            DescriptivePhrase = "Moves the cursor one word to the right";

            ButtonText = DisplayName;
            ToolTip = DisplayName;
            KeyboardBinding = "Text Editor::Alt+Right Arrow";
            ImageIndex = 5;
        }

        public override void Execute()
        {
            CursorRight(false);
        }

        public override bool CanExecute()
        {
            return base.CanExecute() 
                && AddIn.CurrentSelection.Length == 0;
        }
    }
}