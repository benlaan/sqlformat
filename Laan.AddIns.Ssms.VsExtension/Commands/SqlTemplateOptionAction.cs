using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Laan.AddIns.Forms;
using Laan.AddIns.Ssms.VsExtension.Models;
using Laan.AddIns.Ssms.VsExtension.SqlTemplateOption;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class SqlTemplateOptionAction : BaseAction
    {
        public override int CommandId => 4002;

        protected override void Execute()
        {
            var dialogHost = new DialogHost();

            var templates = TemplateDocument.Load();
            var viewModel = new SqlTemplateOptionViewModel(templates);

            if (dialogHost.Show<SqlTemplateOptionView>(viewModel) == DialogResult.OK)
                TemplateDocument.Save(viewModel.Templates.ToList());
        }
    }
}