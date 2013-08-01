using Laan.AddIns.Ssms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Options
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SqlTemplateOptionViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            var templates = TemplateDocument.Load();

            _viewModel = new SqlTemplateOptionViewModel(templates);
            _viewModel.OnSave += viewModel_OnSave;
            _viewModel.OnCancel += _viewModel_OnCancel;
            
            DataContext = _viewModel;
        }

        private void _viewModel_OnCancel(object sender, EventArgs e)
        {
            Close();
        }

        private void viewModel_OnSave(object sender, EventArgs e)
        {
            TemplateDocument.Save(_viewModel.Templates.ToList());
            Close();
        }
    }
}
