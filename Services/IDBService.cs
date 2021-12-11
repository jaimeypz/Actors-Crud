using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace FirstApplication.Services
{
    public interface IDBService
    {

        public DataTable GetDataTable(string option, params string[] parameters);
        public IEnumerable<DataRow> GetData(string option, params string[] parameters);
        public JsonResult GetJsonData(string option, params string[] parameters);
        public string SaveDataTable(DataTable table, string tableName, string[] columns);
        public (string, string) Execute(string option, params string[] parameters);

    }
}
