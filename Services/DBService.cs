using FirstApplication.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FirstApplication.Services
{
    public abstract class DBService :  IDBService
    {
        private readonly string _storedProcedure;
        private readonly string _readConnectionString;
        private readonly string _writeConnectionString;
        public event EventHandler? DataChangeEventHandler;

        public DBService(string readConnectionString, string writeConnectionString, string storedProcedure)
        {
            // Connection string
            _readConnectionString = readConnectionString;
            _writeConnectionString = writeConnectionString;
            _storedProcedure = storedProcedure;
        }

        public DBService(string writeConnectionString, string storedProcedure)
        {
            // Connection string
            _readConnectionString = writeConnectionString;
            _writeConnectionString = writeConnectionString;
            _storedProcedure = storedProcedure;
        }

        public DataTable GetDataTable(string option, params string[] parameters)
        {
            DataTable? dt = default;
            using var connection = new SqlConnection(_readConnectionString);
            connection.Open();
            //
            using SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = _storedProcedure,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 0
            };
            command.Parameters.Add(new SqlParameter("@Option", option));
            command.Parameters.AddRange(parameters.Select((s, i) => new SqlParameter($"@Param{i + 1}", s)).ToArray());
            //command.Parameters.AddRange(parameters.Select((i) => new SqlParameter(i.Key, i.Value)).ToArray());
            //
            using SqlDataAdapter da = new SqlDataAdapter(command);
            using DataSet dataset = new DataSet("Table");
            try
            {
                da.Fill(dataset, "Table");
                if (dataset.Tables.Count > 0)
                {
                    dt = dataset.Tables[0];
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message);
            }
            dt ??= new DataTable();
            return dt;
        }

        public IEnumerable<DataRow> GetData(string option, params string[] parameters) => GetDataTable(option, parameters).AsEnumerable();

        public JsonResult GetJsonData(string option, params string[] parameters) => GetDataTable(option, parameters).ToJson();

        public string SaveDataTable(DataTable table, string tableName, string[] columns)
        {
            var error = "";
            using var connection = new SqlConnection(_writeConnectionString);
            //create object of SqlBulkCopy which help to insert  
            SqlBulkCopy objbulk = new SqlBulkCopy(connection);
            //assign Destination table name  
            objbulk.DestinationTableName = tableName;
            foreach (var c in table.Columns)
            {
                Debug.WriteLine(c);
            }
            foreach (var c in columns)
            {
                objbulk.ColumnMappings.Add(c, c);
            }
            connection.Open();
            try
            {
                //insert bulk Records into DataBase.  
                objbulk.WriteToServer(table);
                // Event notify data change
                OnDataChangeEventHandler();
            }
            catch (SqlException e)
            {
                error = e.Message;
            }
            return error;
        }

        public (string, string) ExecuteSentence(string option, params string[] parameters)
        {
            var id = "";
            var error = "";
            using var connection = new SqlConnection(_writeConnectionString);
            using SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = _storedProcedure;
            command.CommandType = CommandType.StoredProcedure;
            command.Connection.Open();
            command.Parameters.Add(new SqlParameter("@Option", option));
            command.Parameters.AddRange(parameters.Select((s, i) => new SqlParameter($"@Param{i + 1}", s)).ToArray());
            try
            {
                id = command.ExecuteScalar()?.ToString() ?? "";
                // Event notify data change
                OnDataChangeEventHandler();
            }
            catch (SqlException e)
            {
                error = (e.Number == 2627) ? "The record already exists." : e.Message;
            }
            return (id, error);
        }

        public (string, string) Execute(string option, params string[] parameters)
        {
            var upperParameters = parameters.Select(s => s.ToUpperInvariant()).ToArray();
            return ExecuteSentence(option, upperParameters);
        }

        protected virtual void OnDataChangeEventHandler() => DataChangeEventHandler?.Invoke(this, EventArgs.Empty);
    }
}