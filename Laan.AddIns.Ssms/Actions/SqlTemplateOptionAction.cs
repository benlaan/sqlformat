using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;
using Laan.AddIns.Forms;

namespace Laan.AddIns.Ssms.Actions
{
    [MenuBarToolsMenu]
    public class SqlTemplateOptionAction : Core.Action
    {
        /// <summary>
        /// Initializes a new instance of the SqlTemplateOptionAction class.
        /// </summary>
        public SqlTemplateOptionAction(AddIn addIn)
            : base(addIn)
        {
            KeyName = "LaanSqlTemplateOptionView";
            DisplayName = "View Laan Sql Templates";
            DescriptivePhrase = "Viewing Laan Sql Templates";

            ButtonText = "Sql Template Designer";
            ToolTip = "View Laan Sql Templates";
            ImageIndex = 59;
            KeyboardBinding = "Text Editor::Ctrl+Alt+O";
        }

        public override void Execute()
        {
            DialogHost dialogHost = new DialogHost();

            var templates = TemplateDocument.Load();
            var viewModel = new SqlTemplateOptionViewModel(templates);

            if (dialogHost.Show<SqlTemplateOptionView>(viewModel) == DialogResult.OK)
                TemplateDocument.Save(viewModel.Templates.ToList());
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}