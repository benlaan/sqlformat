using System;
using System.Linq;

using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SelectCursorRightWordAction : BaseRightCusorAction
    {
        public SelectCursorRightWordAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSelectCursorRightWordAction";
            DisplayName = "Select Word To Right";
            DescriptivePhrase = "Selects one word to the Right";

            ButtonText = DisplayName;
            ToolTip = DisplayName;
            KeyboardBinding = "Text Editor::Shift+Alt+Right Arrow";
            ImageIndex = 5;
        }

        public override void Execute()
        {
            CursorRight(true);
        }
    }
}