using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;

using EnvDTE;

using Laan.AddIns.Ssms.VsExtension.Utils;

using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

using Task = System.Threading.Tasks.Task;

namespace Laan.AddIns.Ssms.VsExtension
{
    public abstract class BaseAction
    {
        protected static BaseAction Instance { get; set; }

        public abstract int CommandId { get; }
        public virtual Guid CommandSet => new Guid("29b2dad0-1acd-4668-b3e4-ff788e7a4701");

        protected DTE Application { get; set; }

        protected OleMenuCommandService CommandService { get; set; }

        public BaseAction()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Application = (DTE)Package.GetGlobalService(typeof(DTE));
        }

        public static async Task CreateAsync<T>(AsyncPackage package) where T : BaseAction, new()
        {
            await CreateAsync(typeof(T), package);
        }

        public static async Task CreateAsync(Type type, AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var action = Activator.CreateInstance(type) as BaseAction;
            if (action == null)
                throw new Exception(String.Format("Failed to create Action {0} is not a BaseAction descendant", type));

            action.CommandService = commandService;

            var menuCommandId = new CommandID(action.CommandSet, action.CommandId);
            var menuItem = new MenuCommand(action.Execute, menuCommandId);

            commandService.AddCommand(menuItem);

            Instance = action;
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (CanExecute())
            {
                SetStatus("Formatting..", true);
                try
                {
                    var start = DateTime.Now;
                    Execute();
                    SetStatus(string.Format("Completed in {0}", DateTime.Now - start), false);
                }
                catch (Exception ex)
                {
                    SetStatus(String.Format("Error: {0}", ex.Message), false);
                    Debug.WriteLine(ex);
                }
            }
        }

        protected abstract void Execute();

        protected virtual bool CanExecute()
        {
            return true;
        }

        internal TextDocument TextDocument
        {
            get { ThreadHelper.ThrowIfNotOnUIThread(); return (TextDocument)Application.ActiveDocument.Object("TextDocument"); }
        }

        internal string DocumentFullName
        {
            get { ThreadHelper.ThrowIfNotOnUIThread(); return Application.ActiveDocument?.FullName ?? ""; }
        }

        internal bool IsCurrentDocumentExtension(string extension)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return Path.GetExtension(DocumentFullName).Equals(
                String.Format(".{0}", extension),
                StringComparison.CurrentCultureIgnoreCase
            );
        }

        internal void SetStatus(string message, bool animate)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Application.StatusBar.Animate(animate, vsStatusAnimation.vsStatusAnimationBuild);
            Application.StatusBar.Text = message;
        }

        private string GetDumpFile()
        {
            return Path.Combine(Path.GetTempPath(), "Laan.Ssms.log");
        }

        [Conditional("DEBUG")]
        internal void Error(Exception ex)
        {
            try
            {
                File.AppendAllText(GetDumpFile(), ex.ToString() + Environment.NewLine);
            }
            catch (Exception)
            {
            }

            Debug.WriteLine(ex);
        }

        [Conditional("DEBUG")]
        internal void Error(string message, Exception ex)
        {
            try
            {
                File.AppendAllText(GetDumpFile(), message + " : " + ex.ToString() + Environment.NewLine);
            }
            catch (Exception)
            {
            }

            Debug.WriteLine(message + "\n\t" + ex);
        }

        internal void OpenUndoContext(string name, bool strict)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Application.UndoContext.Open(name, strict);
        }

        internal void CloseUndoContext()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Application.UndoContext.Close();
        }

        internal void CancelSelection()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TextDocument.Selection.Cancel();
        }

        internal void SelectCurrentWord()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var cursor = TextDocument.Selection.ActivePoint;
            var point = cursor.CreateEditPoint();
            point.WordLeft(1);
            TextDocument.Selection.MoveToPoint(point, false);
            TextDocument.Selection.WordRight(true, 1);
        }

        internal void ClearSelection()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TextDocument.Selection.Text = "";
        }

        internal void InsertText(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            InsertText(message, true);
        }

        internal void InsertText(string message, bool replace)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (replace)
                ClearSelection();
            TextDocument.Selection.Insert(message, (int)vsInsertFlags.vsInsertFlagsContainNewText);
        }

        internal string CurrentLine
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var cursor = TextDocument.Selection.ActivePoint;
                var startPoint = cursor.CreateEditPoint();
                var endPoint = cursor.CreateEditPoint();
                startPoint.StartOfLine();
                endPoint.EndOfLine();
                return startPoint.GetText(endPoint);
            }
        }

        internal string CurrentWord
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                point.WordLeft(1);
                return point.GetText(cursor).Trim();
            }
        }

        internal string CurrentSelection
        {
            get { ThreadHelper.ThrowIfNotOnUIThread(); return TextDocument.Selection.Text; }
        }


        internal string AllText
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var cursor = TextDocument.StartPoint;
                var point = cursor.CreateEditPoint();
                point.StartOfDocument();
                return point.GetText(TextDocument.EndPoint);
            }
        }

        internal Cursor Cursor
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var point = TextDocument.Selection.TopPoint;
                return new Cursor(point.DisplayColumn, point.Line);
            }
            set
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown(value.Row);
                point.CharRight(value.Column);
                TextDocument.Selection.MoveToPoint(point, false);
            }
        }

        internal Cursor VirtualCursor
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var start = TextDocument.Selection.TextPane.StartPoint;
                var top = TextDocument.Selection.TopPoint;
                return new Cursor(top.DisplayColumn - start.DisplayColumn + 1, top.Line - start.Line + 1);
            }
            set
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown(value.Row);
                point.CharRight(value.Column);
                TextDocument.Selection.MoveToPoint(point, false);
            }
        }


        protected bool PageExists(string category, string page)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                TextDocument.DTE.get_Properties(category, page);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Because SMSS doesn't appear to allow custom properties to be defined, use Registry instead
        protected T ReadConfigValue<T>(string name, T defaultValue)
        {
            using (var laanSoftwareKey = Registry.CurrentUser.CreateSubKey("Laan Software"))
            {
                using (var smssAddinKey = laanSoftwareKey.CreateSubKey("SMSS Addin"))
                {
                    object obj = smssAddinKey.GetValue(name);

                    if (obj != null && smssAddinKey.GetValueKind(name) == RegistryValueKind.String && typeof(T) == typeof(string))
                        return (T)obj;

                    return defaultValue;
                }
            }
        }

        protected bool ReadConfigValue(string name, bool defaultValue)
        {
            using (var laanSoftwareKey = Registry.CurrentUser.CreateSubKey("Laan Software"))
            {
                using (var smssAddinKey = laanSoftwareKey.CreateSubKey("SMSS Addin"))
                {
                    object obj = smssAddinKey.GetValue(name);

                    if (obj != null && smssAddinKey.GetValueKind(name) == RegistryValueKind.DWord)
                        return Convert.ToBoolean(obj);

                    return defaultValue;
                }
            }
        }

        protected void WriteConfigValue<T>(string name, T value)
        {
            using (var laanSoftwareKey = Registry.CurrentUser.CreateSubKey("Laan Software"))
            {
                using (var smssAddinKey = laanSoftwareKey.CreateSubKey("SMSS Addin"))
                {
                    if (typeof(T) == typeof(string))
                        smssAddinKey.SetValue(name, value, RegistryValueKind.String);
                    else if (typeof(T) == typeof(bool))
                        smssAddinKey.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
        }

        protected T ReadProperty<T>(string category, string page, string property, T defaultValue)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {

                var prop = TextDocument.DTE.get_Properties(category, page);
                return (T)prop.Item(property).Value;
            }
            catch (Exception ex)
            {
                Error(String.Format("Failed to ReadProperty('{0}', '{1}', '{2}')", category, page, property), ex);
                return defaultValue;
            }
        }

        protected void WriteProperty<T>(string category, string page, string property, T value)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var prop = TextDocument.DTE.get_Properties(category, page);
                prop.Item(property).Value = value;
            }
            catch (Exception ex)
            {
                Error(String.Format("Failed to WriteProperty('{0}', '{1}', '{2}')", category, page, property), ex);
            }
        }

    }
}
