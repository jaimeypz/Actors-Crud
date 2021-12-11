using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FirstApplication.Extensions
{
    public static class DataTableExtensions
    {
        public static List<Dictionary<string, object>> ToList(this DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            if (dt != null)
            {
                foreach (DataRow? dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn? col in dt.Columns)
                    {
                        if (dr != null && col != null)
                        {
                            var value = Convert.ToString(dr[col]) ?? "";
                            row.Add(col.ColumnName, value);
                        }
                    }
                    rows.Add(row);
                }
            }
            return rows;
        }

        public static JsonResult ToJson(this DataTable dt) => new JsonResult(dt.ToList());
    }
}
