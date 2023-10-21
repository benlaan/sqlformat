using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

using Dapper;

using Laan.AddIns.Ssms.VsExtension.Models;
using Laan.AddIns.Ssms.VsExtension.Utils;

using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class SqlInsertTemplateAction : BaseDropDownAction
    {
        private List<TableDefinition> _definitions;
        private DateTime _lastUpdateTime;

        public class TableDefinition
        {
            public string Schema { get; set; }
            public string Table { get; set; }
            public string Acronym => String.Join("", Regex.Matches(Table, @"[A-Z]").OfType<Match>().Select(m => m.Value));
        }


        public override int CommandId => 4000;

        public SqlInsertTemplateAction()
        {
            Templates = LoadTemplates();
        }

        public List<Template> LoadTemplates()
        {
            try
            {
                return TemplateDocument.Load();
            }
            catch (Exception ex)
            {
                Error(ex);
                return new List<Template>();
            }
        }

        private Cursor DetermineCursorFromBar(IList<string> lines)
        {
            int column = 0;
            int row = 0;

            foreach (string line in lines)
            {
                column = line.IndexOf('|');
                if (column >= 0)
                    break;
                row++;
            }

            // if no bar was found, just put the cursor at the last row and column of the replaced text
            if (column == -1)
            {
                column = lines.Last().Length;
                row = lines.Count() - 1;
            }

            return new Cursor(column, row);
        }

        private SqlConnection GetSqlConnection()
        {
            var scriptFactory = (IScriptFactory)Package.GetGlobalService(typeof(IScriptFactory));
            var info = scriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo;

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = info.ServerName,
                InitialCatalog = info.AdvancedOptions["DATABASE"],
                IntegratedSecurity = info.InMemoryPassword.Length == 0,
                ApplicationName = String.Format("Laan Sql Extension ({0})", info.ApplicationName)
            };

            if (info.InMemoryPassword.Length > 0)
            {
                connectionStringBuilder.Password = new System.Net.NetworkCredential(String.Empty, info.InMemoryPassword).Password;
                connectionStringBuilder.UserID = info.UserName;
            }

            var connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            connection.Open();

            return connection;
        }

        private IList<TableDefinition> GetTableDefinitions()
        {
            const string sql = @"

                SELECT S.Name AS [Schema], T.Name AS [Table]

                FROM sys.tables T

                JOIN sys.schemas S
                  ON S.schema_id = T.schema_id
            ";

            if (_definitions != null && DateTime.Now - _lastUpdateTime < TimeSpan.FromSeconds(30))
                return _definitions;

            using (var sqlConnection = GetSqlConnection())
            {
                _definitions = sqlConnection
                    .Query<TableDefinition>(sql)
                    .ToList()
                    .OrderBy(d => d.Acronym)
                    .ToList();
            }

            _lastUpdateTime = DateTime.Now;
            return _definitions;
        }

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && CurrentSelection == String.Empty;
        }

        public void Expand(string word)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                SelectCurrentWord();
                string padding = new string(' ', 4);
                var foundTemplate = Templates.FirstOrDefault(t => String.Compare(t.Code, word, true) == 0);

                IList<string> raw;

                if (foundTemplate != null)
                {
                    raw = foundTemplate
                        .Body
                        .Split('\n')
                        .Select(line => line.Replace("\t", padding))
                        .ToList();
                }
                else
                {
                    var tableDefinition = GetTableDefinitions().FirstOrDefault(t => String.Compare(t.Acronym, word, true) == 0);
                    if (tableDefinition == null)
                        return;

                    raw = new[] { String.Format("{0}.{1} {2}", tableDefinition.Schema, tableDefinition.Table, tableDefinition.Acronym) };
                }

                if (raw.Count == 0)
                    return;

                var offset = new string(' ', Cursor.Column - 1);

                // add lines, indenting as required, except the first
                var lines = new List<String>(raw.Count);
                lines.Add(raw.First());
                for (int index = 1; index < raw.Count; index++)
                    lines.Add((raw[index] == String.Empty ? String.Empty : offset) + raw[index]);

                var cursor = DetermineCursorFromBar(raw);
                if (cursor.Row < lines.Count)
                    lines[cursor.Row] = lines[cursor.Row].Replace("|", "");

                InsertText(String.Join(Environment.NewLine, lines.ToArray()));
                Cursor = cursor;
            }
            finally
            {
                CancelSelection();
            }
        }

        protected override void ExecuteItem(Item item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Expand(item.Code);
        }

        protected override IEnumerable<Item> GetItems()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var items = new List<Item>();
            var word = CurrentWord.ToLower();

            foreach (var template in Templates.OrderBy(k => k.Code))
            {
                if (template.Code.ToLower().StartsWith(word))
                    items.Add(new Item { Code = template.Code, Name = template.Name });
            }

            try
            {
                var definitions = GetTableDefinitions();
                foreach (var definition in definitions)
                {
                    if (definition.Acronym.ToLower().StartsWith(word))
                        items.Add(new Item { Code = definition.Acronym, Name = String.Format("{0}.{1}", definition.Schema, definition.Table) });
                }
            }
            catch (Exception ex)
            {
                // Ignore - at least show the templates
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return items;
        }

        public static List<Template> Templates { get; set; }
    }
}
