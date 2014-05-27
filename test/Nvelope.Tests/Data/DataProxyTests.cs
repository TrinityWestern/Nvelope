using System.Collections.Generic;
using NUnit.Framework;
using Nvelope.Data;

namespace Nvelope.Tests.Data
{
    [TestFixture]
    public class DataProxyTests
    {
        [Test]
        public void NoOverrides()
        {
            var data = new
            {
                Name = "Brian",
                Id = 168337
            };

            DataProxy d = new DataProxy(new Dictionary<string, string>(), data);
            Assert.AreEqual("Brian", d["Name"]);
            Assert.AreEqual("168337", d["Id"]);
        }

        [Test]
        public void OnlyOverrides()
        {
            var overrides = new Dictionary<string, string>();
            overrides.Add("Name", "Brian");
            overrides.Add("Id", "168337");

            DataProxy d = new DataProxy(overrides);
            Assert.AreEqual("Brian", d["Name"]);
            Assert.AreEqual("168337", d["Id"]);
        }

        [Test]
        public void Mixed()
        {
            var data = new
            {
                Name = "Brian",
                Id = 168337
            };

            var overrides = new Dictionary<string, string>();
            overrides.Add("Color", "Red");

            DataProxy d = new DataProxy(overrides, data);
            Assert.AreEqual("Brian", d["Name"]);
            Assert.AreEqual("168337", d["Id"]);
            Assert.AreEqual("Red", d["Color"]);

            // Now, name should be returned as Bjorn, even though the underlying
            // object still says Brian
            overrides.Add("Name", "Bjorn");
            d = new DataProxy(overrides, data);
            Assert.AreEqual("Bjorn", d["Name"]);
            Assert.AreEqual("168337", d["Id"]);
            Assert.AreEqual("Red", d["Color"]);
        }

        [Test]
        public void KeyNotFound()
        {
            var data = new
            {
                Name = "Brian",
                Id = 168337
            };

            var overrides = new Dictionary<string, string>();
            overrides.Add("Color", "Red");

            DataProxy d = new DataProxy(overrides, data);
            string res = "";
            Assert.Throws(typeof(KeyNotFoundException),() => res = d["Foobar"]);
        }

        [Test (Description="Verifies that the DataProxy class can correctly handle a Dictionary")]
        public void Dictionary()
        {
            var data = new Dictionary<string, object>(){
                { "FirstName", "Bruce" },
                { "LastName", "Wayne" },
                { "Age", 42 }
            };

            var overrides = new Dictionary<string, string>();
            overrides.Add("FirstName", "Batman");

            var d = new DataProxy(overrides, data);
            Assert.AreEqual("Batman", d["FirstName"]);
            Assert.AreEqual("Wayne", d["LastName"]);
            Assert.AreEqual("42", d["Age"]);
        }
    }
}
