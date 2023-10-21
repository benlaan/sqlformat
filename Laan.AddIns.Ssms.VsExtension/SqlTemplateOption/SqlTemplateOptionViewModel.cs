using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Laan.AddIns.Forms;
using Laan.AddIns.Ssms.VsExtension.Models;

namespace Laan.AddIns.Ssms.VsExtension.SqlTemplateOption
{
    public class SqlTemplateOptionViewModel : INotifyPropertyChanged, IDialog
    {
        private string _filterText;
        private bool _isDirty;
        private Template _selectedTemplate;
        private readonly List<Template> _originalTemplates;

        public SqlTemplateOptionViewModel(List<Template> templates)
        {
            _filterText = string.Empty;
            _originalTemplates = templates;

            Templates = new ObservableCollection<Template>(_originalTemplates);

            Save = new DelegateCommand(ExecuteSave, IsDirty);
            Cancel = new DelegateCommand(ExecuteCancel);
            Add = new DelegateCommand(ExecuteAdd);
            Duplicate = new DelegateCommand(ExecuteDuplicate);
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
            FilterText = String.Empty;
            _isDirty = false;

            OnSave?.Invoke(this, EventArgs.Empty);
        }

        private void ExecuteCancel()
        {
            if (_isDirty)
            {
                var confirmCancel = "There are unsaved changes - do you want to continue?";

                if (MessageBox.Show(confirmCancel, "Confirm Cancel changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                _isDirty = false;
            }

            OnCancel?.Invoke(this, EventArgs.Empty);
        }

        private void AddNew(Template newTemplate)
        {
            _originalTemplates.Add(newTemplate);
            Templates.Add(newTemplate);
            AssignPropertyChangedHandler(new[] { newTemplate });

            SelectedTemplate = newTemplate;
            MarkAsDirty();
        }

        private void ExecuteAdd()
        {
            AddNew(new Template { Name = "New Template" });
        }

        private void ExecuteDuplicate()
        {
            if (SelectedTemplate == null)
                return;

            AddNew(SelectedTemplate.Clone());
        }

        private void ExecuteRemove()
        {
            var indexOfRemovedItem = Templates.IndexOf(SelectedTemplate);
            Templates.Remove(SelectedTemplate);
            _originalTemplates.Remove(SelectedTemplate);

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
            return string.IsNullOrEmpty(_filterText)
                || template.Code.ToLower().Contains(filterText)
                || template.Name.ToLower().Contains(filterText);
        }

        private void ReselectPreviousItem(int selectedIndex)
        {
            if (!Templates.Any())
                return;

            var index = Templates.IndexOf(SelectedTemplate);

            if (index >= 0)
                SelectedTemplate = Templates[index];
            else
                SelectedTemplate = Templates[Clamp(selectedIndex, 0, Templates.Count - 1)];
        }

        private void NotifyPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
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
        public DelegateCommand Duplicate { get; set; }

        public bool CanCancel
        {
            get { return !_isDirty; }
        }
    }
}
