using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using Nvelope;

namespace Nvelope.Tests
{
    [TestFixture]
    public class DataTableExtensionTests
    {
        private DataTable _table;
        protected DataTable Table
        {
            get
            {
                if (_table == null)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("A", typeof(string));
                    dt.Columns.Add("B", typeof(int));
                    dt.Columns.Add("C", typeof(bool));
                    var row = dt.NewRow();
                    row["A"] = "brian";
                    row["B"] = 2011;
                    row["C"] = true;
                    dt.Rows.Add(row);
                    _table = dt;
                }
                return _table;
            }
        }

        [Test]
        public void ToDictionary()
        {
            var dict = Table.Rows[0].ToDictionary();
            Assert.AreEqual("([A,brian],[B,2011],[C,True])", dict.Print());
        }

        [Test]
        public void Print()
        {
            Assert.AreEqual("(([A,brian],[B,2011],[C,True]))", Table.Print());
            Assert.AreEqual("([A,brian],[B,2011],[C,True])", Table.Rows[0].Print());
        }

        [Test]
        public void ToListDataRow()
        {
           IEnumerable<DataRow> tList = Table.Rows.ToList();
           var x = tList.Count();
           var y = Table.Rows.Count;
           Assert.AreEqual(x, y);
            foreach (var r in tList)
            {
                Assert.AreEqual(typeof(DataRow), r.GetType());
            }
            
        }

        [Test]
        public void ToListDataCollection()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(Table);
            var t = ds.Tables.ToList();
            var x = t.Count();
            Assert.AreEqual(1, x);
            foreach (var y in t)
            {
                Assert.AreEqual(typeof(DataTable), y.GetType());
            }
        }
    }
}
