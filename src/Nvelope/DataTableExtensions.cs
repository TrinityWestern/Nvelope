using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nvelope.Tabular;

namespace Nvelope
{
    public static class DataTableExtensions
    {
        public static Dictionary<string, object> ToDictionary(this DataRow dr)
        {
            var res = new Dictionary<string, object>();
            foreach (DataColumn col in dr.Table.Columns)
                res.Add(col.ColumnName, dr[col]);
            return res;
        }

        public static IEnumerable<DataRow> ToList(this DataRowCollection dc)
        {
            foreach (DataRow row in dc)
                yield return row;
        }

        public static IEnumerable<DataTable> ToList(this DataTableCollection dt)
        {
            foreach (DataTable t in dt)
                yield return t;
        }

        public static string Value(this DataTable dt, CellLoc loc)
        {
            return dt.Rows[loc.Row][loc.Col].ConvertTo<string>();
        }

        public static HashSet<string> Tablenames(this DataSet ds)
        {
            var res = new HashSet<string>();
            foreach (DataTable table in ds.Tables)
                res.Add(table.TableName);
            return res;
        }

        public static DataTable Table(this DataSet ds, string tablename)
        {
            if (ds.Tables.Contains(tablename))
                return ds.Tables[tablename];
            else
                return null;
        }

        public static HashSet<string> Columnnames(this DataTable dt)
        {
            var res = new HashSet<string>();
            foreach (DataColumn col in dt.Columns)
                res.Add(col.ColumnName);
            return res;
        }
    }
}
