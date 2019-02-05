using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataMapping
{
    public class ColumnManager : IColumnManager
    {
        public ParameterManager ParameterManager { get; private set; }

        private IEnumerable _dataList;
        public IEnumerable DataList
        {
            get => _dataList;
            set
            {
                _dataList = value;
                ParameterManager = new ParameterManager();
                _columns = CreateColumns();
            }
        }

        private DataTable _dbDataTable;
        public DataTable DbDataTable => _dbDataTable ?? (_dbDataTable = GetDbDataTable());

        
        private DataTable _viewDataTable;
        public DataTable ViewDataTable => _viewDataTable ?? (_viewDataTable = GetViewDataTable());

        private List<Column> _columns;
        internal IEnumerable<Column> Columns => _columns;

        internal Column this[string name] => _columns.Find(c => c.Header == name);

        private List<Column> CreateColumns()
        {
            const int maxExamples = 5;
            var columns = new List<Column>();
            var dataTable = ((DataView)DataList).Table;
            foreach (DataColumn col in dataTable.Columns)
            {
                columns.Add(new Column()
                {
                    Header = col.ColumnName,
                    Parameter = ParameterManager[col.ColumnName]?.Name ?? ParameterManager.Default,
                });
            }
            List<string> examples;
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var col in columns)
                {
                    examples = col.ExampleValues;
                    if (examples.Count <= maxExamples && !string.IsNullOrEmpty((string)row[col.Header]) && examples.All(e => e != (string)row[col.Header]))
                        examples.Add((string)row[col.Header]);
                }
                if (columns.All(e => e.ExampleValues.Count == maxExamples + 1)) break;
            }
            foreach (var col in columns)
            {
                examples = col.ExampleValues;
                if (examples.Count != maxExamples + 1) continue;
                examples.RemoveAt(maxExamples);
                examples.RemoveAt(maxExamples - 1);
                examples.Insert(maxExamples - 1, "...");
            }
            return columns;
        }

        internal void SetPapameters(Dictionary<string, string> parameters)
        {
            foreach (var col in parameters)
            {
                _columns.Find(c => c.Header == col.Key).Parameter = col.Value;
            }
        }

        internal string[] ValidationParameters()
            => ParameterManager.ValidationParameters(_columns.Select(c =>
            {
                c.DataErrors.Clear();
                return c.Parameter;
            }).ToArray());

        internal bool IsValidationData()
        {
            var result = true;
            var table = ((DataView)_dataList).Table;
            foreach (var col in _columns)
            {
                col.DataErrors.Clear();
                if (col.Parameter == ParameterManager.Ignore) continue;
                var papameter = ParameterManager[col.Parameter];

                if (!papameter.IsAllowEmtyValues && table.AsEnumerable().Select(r => r.Field<string>(col.Header)).Contains(string.Empty))
                {
                    col.DataErrors.Add("Есть пустые значения");
                }
                if (papameter.Type == DataType.Decimal)
                    try
                    {
                        foreach (var r in table.AsEnumerable().Select(r => r.Field<string>(col.Header)))
                        {
                            decimal.Parse(r.Replace('.', ','));
                        }
                    }
                    catch (FormatException)
                    {
                        col.DataErrors.Add("Есть не числовые значения");
                    }

                result = result && !col.DataErrors.Any();
            }

            var custUnique = new UniqueConstraint(ParameterManager.UniqueConstraint.Select(u => _columns.Find(c => c.Parameter == u).Header)
                .Select(c => table.Columns[c]).ToArray());
            try
            {
                table.Constraints.Add(custUnique);
                table.Constraints.Clear();
            }
            catch (ArgumentException)
            {
                foreach (var u in ParameterManager.UniqueConstraint)
                {
                    _columns.Find(c => c.Parameter == u)
                        .DataErrors.Add($"\"{ParameterManager.UniqueConstraint.First()}\" + \"{ParameterManager.UniqueConstraint.Last()}\" не уникальны!");
                }
                result = false;
            }
            return result;
        }

        private DataTable GetViewDataTable()
        {
            var dbHeaders = _columns.FindAll(col => !ParameterManager.IsIgnore(col.Parameter) && !ParameterManager.IsDefault(col.Parameter))
                .Select(col => col.Header).ToArray();
           return ((DataView)DataList).ToTable(false, dbHeaders);
        }

        private DataTable GetDbDataTable()
        {
            var dbTable = new DataTable(ViewDataTable.TableName);
            foreach (DataColumn col in ViewDataTable.Columns)
            {
                var dbColumn = new DataColumn();
                dbTable.Columns.Add(dbColumn);
                SetDbColumnName(dbColumn, col.ColumnName);
                
                dbColumn.DataType = ParameterManager[this[col.ColumnName].Parameter].Type == DataType.Decimal ? typeof(decimal) : typeof(string);
                dbColumn.AllowDBNull = ParameterManager[this[col.ColumnName].Parameter].IsAllowEmtyValues;
            }
            foreach (DataRow row in ViewDataTable.Rows)
            {
                for (var i = 0; i < dbTable.Columns.Count; i++)
                {
                    if (dbTable.Columns[i].DataType == typeof(decimal))
                        row[i] = row[i].ToString().Replace('.', ',');
                }
                dbTable.Rows.Add(row.ItemArray);
            }
            
            dbTable.AcceptChanges();
            return dbTable;
        }

        private void SetDbColumnName(DataColumn column, string columnName, int prefixName = 1)
        {
            try
            {
                column.ColumnName = ParameterManager[this[columnName].Parameter].IsRepeat 
                    ? $"{this[columnName].Parameter}{prefixName}" : this[columnName].Parameter;
            }
            catch (DuplicateNameException e)
            {
                SetDbColumnName(column, columnName, ++prefixName);
            }
        }
    }
}