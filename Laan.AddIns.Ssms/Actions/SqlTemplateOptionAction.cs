using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;
using Laan.AddIns.Forms;

namespace Laan.AddIns.Ssms.Actions
{
    [Menu("Laan")]
    public class SqlTemplateOptionAction : Core.BaseAction
    {
        /// <summary>
        /// Initializes a new instance of the SqlTemplateOptionAction class.
        /// </summary>
        public SqlTemplateOptionAction(AddIn addIn)
            : base(addIn)
        {
            KeyName = "LaanSqlTemplateOptionView";
            DisplayName = "Edit Sql Templates";
            DescriptivePhrase = "Editing Sql Templates";

            ButtonText = "Sql Template Designer";
            ToolTip = "Edit Sql Templates";
            KeyboardBinding = "Global::Ctrl+Alt+O";
        }

        public override void Execute()
        {
            var dialogHost = new DialogHost();

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