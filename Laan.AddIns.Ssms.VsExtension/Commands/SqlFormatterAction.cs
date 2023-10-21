using System;

using Laan.AddIns.Ssms.VsExtension.Utils;
using Laan.Sql.Formatter;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class SqlFormatterAction : BaseAction
    {
        public override int CommandId => 4001;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            using (var undo = new ScopedUndoContext(this, "SqlFormatter"))
            {
                var textDocument = TextDocument;

                if (textDocument.Selection.IsEmpty)
                    textDocument.Selection.SelectAll();

                try
                {
                    var engine = new FormattingEngine();
                    InsertText(engine.Execute(textDocument.Selection.Text) + Environment.NewLine);
                }
                finally
                {
                    textDocument.Selection.Cancel();
                }
            }
        }
    }
}
