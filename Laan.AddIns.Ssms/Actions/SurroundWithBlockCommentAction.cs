using System;
using System.Linq;

using EnvDTE;
using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SurroundWithBlockCommentAction : Core.BaseAction
    {
        public SurroundWithBlockCommentAction(Core.AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSurroundWithBlockCommentAction";
            DisplayName = "Surround Selection With Comments";
            DescriptivePhrase = "Surrounds Selection With Comments";

            ButtonText = "Surround Selection With Block Comments";
            ToolTip = "Surround Selection With Block Comments";
            KeyboardBinding = "Text Editor::/";
            ImageIndex = 6;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;

            if (AddIn.CurrentSelection.Length == 0)
                textDocument.Selection.Insert("/");
            else
            {
                var ranges = textDocument.Selection.TextRanges
                    .OfType<TextRange>()
                    .Select(tr => new
                    {
                        Start = new Cursor(tr.StartPoint.DisplayColumn, tr.StartPoint.Line),
                        End = new Cursor(tr.EndPoint.DisplayColumn, tr.EndPoint.Line),
                        StartPoint = tr.StartPoint,
                        EndPoint = tr.EndPoint
                    })
                    .Where(r => !r.Start.Equals(r.End))
                    .ToList();

                if (ranges.Any(r => r.StartPoint.AtStartOfLine))
                {
                    var text = textDocument.Selection.Text;
                    if (ranges.Count > 1)
                    {
                        var trailingNewLine = textDocument.Selection.ActivePoint.AtStartOfLine ? Environment.NewLine : String.Empty;
                        AddIn.InsertText(String.Format("/*{0}{1}{0}*/{2}", Environment.NewLine, text.Trim(), trailingNewLine), true);
                    }
                    else
                        AddIn.InsertText(String.Format("/* {0} */" + Environment.NewLine, text.TrimEnd()), true);
                }
                else
                    foreach (var range in ranges)
                    {
                        var editPoint = range.StartPoint.CreateEditPoint();
                        var text = editPoint.GetText(range.EndPoint);

                        editPoint.ReplaceText(
                            range.EndPoint,
                            String.Format(@"/* {0} */", text),
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
