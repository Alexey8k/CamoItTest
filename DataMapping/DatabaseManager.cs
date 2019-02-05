using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapping
{
    public class DatabaseManager
    {
        public DatabaseManager(ColumnManager columnManager)
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultDbConnectionStr"].ConnectionString);
            _columnManager = columnManager;
        }

        private string _dbNameValid = string.Empty;

        private readonly SqlConnection _connection;

        private readonly ColumnManager _columnManager;

        public void CreateDb(string fileName)
        {
            try
            {
                _connection.Open();
                var databases = _connection.GetSchema("databases").AsEnumerable();
                var dbNameCorrect = fileName.Trim().Replace(" ", "_").Split('.').First();    // guard against SQL injection
                _dbNameValid = CreateDbName(databases, dbNameCorrect);
                var commandStr = $"CREATE DATABASE [{_dbNameValid}]";
                var command = new SqlCommand(commandStr, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("База данных не была создана. Ошибка подключения.");
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }

        public void CopyDataToDb()
        {
            var dbDataTable = _columnManager.DbDataTable;
            var tablename = CreateDbTable(dbDataTable);
            try
            {
                _connection.Open();
                using (var bulkCopy = new SqlBulkCopy(_connection))
                {
                    //bulkCopy.DestinationTableName = $"[{_dbNameValid}].[dbo].[{tablename}]";
                    bulkCopy.DestinationTableName = $"[dbo].[{tablename}]"; // for db Azure

                    bulkCopy.WriteToServer(dbDataTable);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка копирования данных в базу данных.");
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }

        } 

        private string CreateDbTable(DataTable dbDataTable)
        {
            var tableName = CreateTableName($"Table_{dbDataTable.TableName}");
            var commandStr = new StringBuilder();
            //commandStr.AppendLine($"USE [{_dbNameValid}]");
            commandStr.AppendLine($"CREATE TABLE [{tableName}](");
            foreach (DataColumn col in dbDataTable.Columns)
            {
                commandStr.AppendLine(
                    $"[{col.ColumnName}] {(col.DataType == typeof(decimal) ? "decimal(18,2)" : "varchar(129)")} {(col.AllowDBNull ? "null" : "not null")},");
            }
            commandStr.Append(')');
            var command = new SqlCommand(commandStr.ToString(), _connection);

            try
            {
                _connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                new Exception("Не удалоси добавить таблицу в базу данных.");
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return tableName;
        }

        private string CreateTableName(string tableName)
        {
            var tableNames = new List<string>();
            try
            {
                _connection.Open();
                using (var reader = new SqlCommand
                {
                    Connection = _connection,
                    //CommandText = $"USE [{_dbNameValid}] SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
                    CommandText = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"   // for db Azure
                }.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }
            catch
            {
                throw new Exception("Неудалось создать таблицу в базе данных.");
            }
            finally
            {
                _connection.Close();
            }
            return HelperCreateTableName(tableNames, tableName);
        }

        private string HelperCreateTableName(List<string> tableNames, string tableName, int? postfixName = null)
            => tableNames.Contains($"{tableName}{postfixName}") 
            ? HelperCreateTableName(tableNames, tableName, (postfixName ?? 0) + 1)
            : $"{tableName}{postfixName}";



        private static string CreateDbName(IEnumerable<DataRow> databases, string dbName, int? postfixName = null)
            => databases.Any(r => (string)r["database_name"] == $"{dbName}{postfixName}")
                ? CreateDbName(databases, dbName, (postfixName ?? 0) + 1)
                : $"{dbName}{postfixName}";


    }
}
