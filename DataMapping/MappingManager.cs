using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataMapping
{
    public class MappingManager : IMappingManager    // ToDo: интерфейс переделать на супер класс 
    {
        public MappingManager(IEnumerable dataList, string fileName)
        {
            DataList = dataList;
            FileName = fileName;
        }

        public string FileName { get; }

        public IEnumerable DataList
        {
            get => ((ColumnManager)GetColumnManager()).DataList;
            set
            {
                ((ColumnManager) GetColumnManager()).DataList = value;
                _databaseManager = new DatabaseManager((ColumnManager)GetColumnManager());
            }
        }

        private DatabaseManager _databaseManager;

        public IEnumerable<Column> Columns => ((ColumnManager) GetColumnManager()).Columns;

        private IColumnManager _columnManager;
        private IColumnManager GetColumnManager()
        {
            return _columnManager ?? (_columnManager = new ColumnManager());
        }

        public IEnumerable<string> Parameters => ((ColumnManager) GetColumnManager()).ParameterManager.GetNames();

        public void SetParameters(Dictionary<string, string> parameters) => ((ColumnManager)GetColumnManager()).SetPapameters(parameters);

        public string[] ValidationParameters() => ((ColumnManager)GetColumnManager()).ValidationParameters();

        public bool IsValidationData() => ((ColumnManager) GetColumnManager()).IsValidationData();

        public void CreateDb() => _databaseManager.CreateDb(((DataView)DataList).Table.TableName);

        public void CopyDataToDb() => _databaseManager.CopyDataToDb();

        public DataTable ViewDataTable => ((ColumnManager) GetColumnManager()).ViewDataTable;
    }
}