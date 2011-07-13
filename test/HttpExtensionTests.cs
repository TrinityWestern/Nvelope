using System;
using System.Collections.Generic;
using System.Web;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class HttpExtensionTests
    {
        protected HttpRequest mock;
        protected HttpRequest mock_date;
        protected HttpRequest mock_checked;
        protected HttpRequest mock_handle;

        [TestFixtureSetUp]
        public void Setup()
        {
            mock = new HttpRequest("lolwat", "http://example.org", "foo=bar&x=on");
            mock_date = new HttpRequest("lolwat", "http://example.org", "birthday=1234&d=2001-04-23");
            mock_checked = new HttpRequest("lolwat", "http://example.org", "checked=checked");
            mock_handle = new HttpRequest("lolwat", "http://example.org", "check=check&list=1,2,3");
        }
        [Test]
        public void RequiredParamTest()
        {
            Assert.Throws(typeof(HttpException), () => mock.RequiredParam<int>("dingdong"));
        }
        [Test]
        public void OptionalParamTest()
        {
            Assert.AreEqual("bar", mock.OptionalParam<string>("foo", ""));
            Assert.Throws(typeof(HttpException), () => mock.OptionalParam<int>("foo", 0));
            Assert.IsEmpty(mock.OptionalParam<string>("baz", ""));
            Assert.IsNull(mock.OptionalParam<int?>("dingdong", null));

            Assert.AreEqual("on", mock.OptionalParam<string>("x", ""));
            Assert.IsTrue(mock.OptionalParam<bool>("x", false));
            Assert.IsFalse(mock.OptionalParam<bool>("y", false));

            Assert.IsNull(mock_date.OptionalParam<DateTime?>("birthday", null));
            Assert.AreEqual(new DateTime(2001, 4, 23), mock_date.OptionalParam<DateTime?>("d", null));
        }
        [Test]
        public void CheckboxParam()
        {
            Assert.IsTrue(mock_checked.CheckBoxParam("checked"));
            Assert.IsFalse(mock.CheckBoxParam("checked"));
        }
        [Test]
        public void HasParamTest()
        {
            Assert.IsTrue(mock.HasParam("x"));
            Assert.IsFalse(mock.HasParam("dingdong"));
        }
        [Test]
        public void HandleTest()
        {
            List<string> l = new List<string>();
            l.Add("1");
            l.Add("2");
            l.Add("3");
            Assert.AreEqual(l, mock_handle.HandleParam("list", HttpRequestHandlers.CommaList));
        }
    }
}
