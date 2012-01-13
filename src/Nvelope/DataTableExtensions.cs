using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nvelope.Tabular;

namespace Nvelope
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Converts a DataRow to a dictionary
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnsToInclude"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this DataRow dr, IEnumerable<string> columnsToInclude = null)
        {
            columnsToInclude = columnsToInclude ?? dr.Table.Columnnames();

            var res = new Dictionary<string, object>();
            foreach (DataColumn col in dr.Table.Columns)
                if(columnsToInclude.Contains(col.ColumnName))
                    res.Add(col.ColumnName, dr[col]);
            return res;
        }

        /// <summary>
        /// Converts a Datatable to a list or Dictionaries
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnsToInclude"></param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> ToDictionaries(this DataTable dt, IEnumerable<string> columnsToInclude = null)
        {
            if (dt == null)
                return new List<Dictionary<string, object>>();

            return dt.Rows.ToList().Select(r => r.ToDictionary(columnsToInclude)).ToList();
        }

        public static IEnumerable<DataRow> ToList(this DataRowCollection dc)
        {
            foreach (DataRow row in dc)
                yield return row;
        }

        public static IEnumerable<DataRow> Rows(this DataTable dt)
        {
            if (dt != null)
                foreach (DataRow dr in dt.Rows)
                    yield return dr;
        }

        public static IEnumerable<DataTable> ToList(this DataTableCollection dt)
        {
            foreach (DataTable t in dt)
                yield return t;
        }

        public static IEnumerable<DataTable> Tables(this DataSet ds)
        {
            if (ds != null)
                foreach (DataTable dt in ds.Tables)
                    yield return dt;
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

        /// <summary>
        /// Gets a table, if it exists in the dataset. If it doesn't exist, return null
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
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
