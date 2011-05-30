using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nvelope.Reflection;

namespace Nvelope.Data
{
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// Get all the columns that are in the reader
        /// </summary>
        public static IEnumerable<string> GetReaderFields(this IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                yield return reader.GetName(i);
        }
 
        /// <summary>
        /// Get a collection of dictionaries of all the rows in the reader
        /// </summary>
        public static List<Dictionary<string, object>> AllRows(this IDataReader reader)
        {
            var fields = reader.GetReaderFields();
            var res = new List<Dictionary<string, object>>();
            while (reader.Read())
                res.Add(_getRow(reader, fields));

            return res;
        }

        /// <summary>
        /// Gets the single value returned by the SQL query as the specified type
        /// </summary>
        public static T SingleValue<T>(this IDataReader reader)
        {
            return reader.SingleColumn<T>().Single();
        }

        /// <summary>
        /// Reads the single column (or just the first column) from the reader, and
        /// converts the values to the specified type
        /// </summary>
        public static List<T> SingleColumn<T>(this IDataReader reader)
        {
            List<T> res = new List<T>();
            while(reader.Read())
                res.Add(reader[0].ConvertTo<T>());
            return res;

        }
        /// <summary>
        /// Convert all the rows in the reader to objects
        /// </summary>
        public static IEnumerable<T> ReadAll<T>(this IDataReader reader) where T : class, new()
        {
            var converter = new ObjectReader<T>() { TrimFields = true };
            return converter.ReadAll(reader.AllRows());
        }

        #region Helpers

        private static Dictionary<string, object> _getRow(IDataReader source, IEnumerable<string> fields)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            foreach (string field in fields)
                res.Add(field, source[field]);
            return res;
        }
        #endregion

    }
}
