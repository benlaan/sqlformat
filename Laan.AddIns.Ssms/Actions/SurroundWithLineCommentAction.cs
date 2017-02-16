using System;
using System.Linq;

using EnvDTE;
using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SurroundWithLineCommentAction : Core.BaseAction
    {
        public SurroundWithLineCommentAction(Core.AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSurroundWithLineCommentAction";
            DisplayName = "Surround Selection With Comments";
            DescriptivePhrase = "Surrounds Selection With Comments";

            ButtonText = "Surround Selection With Line Comments";
            ToolTip = "Surround Selection With Line Comments";
            KeyboardBinding = "Text Editor::-";
            ImageIndex = 6;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;

            if (AddIn.CurrentSelection.Length == 0)
                textDocument.Selection.Insert("-");
            else
            {
                var ranges  = textDocument.Selection.TextRanges
                    .OfType<TextRange>()
                    .Select(tr => new 
                    {
                        Start = new Cursor(tr.StartPoint.DisplayColumn, tr.StartPoint.Line),
                        End   = new Cursor(tr.EndPoint.DisplayColumn, tr.EndPoint.Line),
                        StartPoint  = tr.StartPoint,
                        EndPoint = tr.EndPoint
                    })
                    .ToList();

                foreach (var range in ranges)
                {
                    var editPoint = range.StartPoint.CreateEditPoint();
                    var text = editPoint.GetText(range.EndPoint);
                    
                    editPoint.ReplaceText(
                        range.EndPoint, 
                        String.Format(@"-- {0}", text),
                        (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat
                    );
                }
                
                textDocument.Selection.CharRight(false, textDocument.Selection.AnchorPoint.DisplayColumn);
            }
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql");
        }
    }
}
