using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ASP_NET_MVC_KP_TestCamoIt_.Auxiliary_Logic
{
    internal class CsvDataSource : DataSource
    {
        public CsvDataSource(HttpPostedFileBase sourceFile)
        {
            DataList = ExecuteSelect(sourceFile);
        }

        public IEnumerable DataList { get; }

        private IEnumerable ExecuteSelect(HttpPostedFileBase sourceFile)
        {
            IEnumerable dataList = null;
            if (sourceFile != null)
            {
                var data = new DataTable { TableName = Path.GetFileName(sourceFile.FileName).Trim().Replace(" ", "_").Split('.').First() };

                using (var sr = new StreamReader(sourceFile.InputStream))
                {
                    var s = "";
                    var dataValues = sr.ReadLine().Split(',');

                    foreach (string token in dataValues)
                    {
                        var col = new DataColumn(token, typeof(string));
                        if (!Regex.IsMatch(token, @"^[A-Za-z\d_]+$"))
                            throw new DataException("Файл содержит не допустимые названия колонок");
                        data.Columns.Add(col);
                    }

                    while ((s = sr.ReadLine()) != null)
                    {
                        dataValues = s.Split(',');
                        data.Rows.Add(CopyRowData(dataValues, data.NewRow()));
                    }
                }
                data.AcceptChanges();
                dataList = new DataView(data); ;
            }
            else
            {
                throw new System.Configuration.ConfigurationErrorsException("Не установлен источник данных.");
            }
            if (null == dataList)
            {
                throw new InvalidOperationException("Данные небыли загружены из источника.");
            }
            return dataList;
        }

        private DataRow CopyRowData(string[] source, DataRow target)
        {
            try
            {
                for (int i = 0; i < source.Length; i++)
                {
                    target[i] = source[i];
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                return target;
            }
            return target;
        }
    }
}