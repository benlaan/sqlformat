using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Laan.AddIns.Forms;

namespace Laan.AddIns.Ssms.Actions
{
    public class SqlTemplateOptionViewModel : INotifyPropertyChanged, IDialog
    {
        private string _filterText;
        private bool _isDirty;
        private Template _selectedTemplate;
        private readonly List<Template> _originalTemplates;
                
        /// <summary>
        /// Initializes a new instance of the SqlTemplateOptionViewModel class.
        /// </summary>
        /// <param name="templates"></param>
        public SqlTemplateOptionViewModel(List<Template> templates)
        {
            _filterText = String.Empty;
            _originalTemplates = templates;

            Templates = new ObservableCollection<Template>(_originalTemplates);

            Save = new DelegateCommand(ExecuteSave, IsDirty);
            Cancel = new DelegateCommand(ExecuteCancel);
            Add = new DelegateCommand(ExecuteAdd);
            Remove = new DelegateCommand(ExecuteRemove, () => SelectedTemplate != null);

            Templates.CollectionChanged += CollectionChanged;
            AssignPropertyChangedHandler(Templates);
            SelectedTemplate = Templates.FirstOrDefault();
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs ev)
        {
            MarkAsDirty();
        }

        private void AssignPropertyChangedHandler(IList newItems)
        {
            if (newItems == null)
                return;

            foreach (var item in newItems.OfType<INotifyPropertyChanged>())
                item.PropertyChanged += ItemPropertyChanged;
        }

        private void CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            Remove.RaiseCanExecuteChanged();

            AssignPropertyChangedHandler(e.NewItems);

            if (e.OldItems == null)
                return;

            foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                item.PropertyChanged -= ItemPropertyChanged;
        }

        private bool IsDirty()
        {
            return _isDirty;
        }

        private void ExecuteSave()
        {
            _isDirty = false;
            if (OnSave != null)
                OnSave(this, EventArgs.Empty);
        }

        private void ExecuteCancel()
        {
            if (_isDirty)
            {
                string confirmCancel = "There are unsaved changes - are you sure you wish to cancel?";

                if (MessageBox.Show(confirmCancel, "Confirm Cancel changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                _isDirty = false;
            }

            if (OnCancel != null)
                OnCancel(this, EventArgs.Empty);
        }

        private void ExecuteAdd()
        {
            Template newTemplate = new Template() { Name = "New Template" };
            Templates.Add(newTemplate);
            SelectedTemplate = newTemplate;
            MarkAsDirty();
        }

        private void ExecuteRemove()
        {
            var indexOfRemovedItem = Templates.IndexOf(SelectedTemplate);
            if (indexOfRemovedItem >= 0)
                Templates.RemoveAt(indexOfRemovedItem);

            if (Templates.Count > 0)
                SelectedTemplate = Templates[Clamp(indexOfRemovedItem, 0, Templates.Count - 1)];

            MarkAsDirty();
        }

        private void MarkAsDirty()
        {
            _isDirty = true;
            Save.RaiseCanExecuteChanged();
        }

        private int Clamp(int item, int minimum, int maximum)
        {
            if (item < minimum)
                return minimum;

            if (item > maximum)
                return maximum;

            return item;
        }

        private bool IsTemplatedSelectedByFilter(string filterText, Template template)
        {
            return String.IsNullOrEmpty(_filterText)
                || template.Code.ToLower().Contains(filterText)
                || template.Name.ToLower().Contains(filterText);
        }

        private void ReselectPreviousItem(int selectedIndex)
        {
            if (!Templates.Any())
                return;

            int index = Templates.IndexOf(SelectedTemplate);
            
            if (index >= 0)
                SelectedTemplate = Templates[index];
            else
                SelectedTemplate = Templates[Clamp(selectedIndex, 0, Templates.Count - 1)];
        }

        private void NotifyPropertyChanged(string memberName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
        }

        public ObservableCollection<Template> Templates { get; set; }

        public Template SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate == value)
                    return;

                _selectedTemplate = value;

                NotifyPropertyChanged("SelectedTemplate");
                Remove.RaiseCanExecuteChanged();
            }
        }

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                if (_filterText == value)
                    return;

                _filterText = value;

                var selectedIndex = SelectedTemplate != null ? Templates.IndexOf(SelectedTemplate) : 0;
                var filterText = _filterText.ToLower();

                Templates.Clear();
                foreach (Template template in _originalTemplates)
                {
                    if (IsTemplatedSelectedByFilter(filterText, template))
                        Templates.Add(template);
                }

                ReselectPreviousItem(selectedIndex);

                NotifyPropertyChanged("FilterText");
                NotifyPropertyChanged("Templates");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnSave;
        public event EventHandler OnCancel;

        public DelegateCommand Save { get; set; }
        public DelegateCommand Cancel { get; set; }
        public DelegateCommand Add { get; set; }
        public DelegateCommand Remove { get; set; }

        public bool CanCancel
        {
            get { return !_isDirty; }
        }
    }
}
