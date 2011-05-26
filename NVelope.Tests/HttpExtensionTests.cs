using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nvelope;
using System.Web;

namespace Nvelope.Tests
{
    [TestFixture]
    public class HttpExtensionTests
    {
        protected HttpRequest mock;
        protected HttpRequest mock_date;
        protected HttpRequest mock_handle;

        [TestFixtureSetUp]
        public void Setup()
        {
            mock = new HttpRequest("lolwat", "http://example.org", "foo=bar&x=on");
            mock_date = new HttpRequest("lolwat", "http://example.org", "birthday=1234&d=2001-04-23");
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
            Assert.AreEqual("", mock.OptionalParam<string>("baz", ""));
            Assert.AreEqual(null, mock.OptionalParam<int?>("dingdong", null));

            Assert.AreEqual("on", mock.OptionalParam<string>("x", ""));
            Assert.AreEqual(true, mock.OptionalParam<bool>("x", false));
            Assert.AreEqual(false, mock.OptionalParam<bool>("y", false));

            Assert.AreEqual(null, mock_date.OptionalParam<DateTime?>("birthday", null));
            Assert.AreEqual(new DateTime(2001, 4, 23), mock_date.OptionalParam<DateTime?>("d", null));
        }
        [Test]
        public void HasParamTest()
        {
            Assert.AreEqual(true, mock.HasParam("x"));
            Assert.AreEqual(false, mock.HasParam("dingdong"));
        }
        [Test]
        public void HandleTest()
        {
            Assert.AreEqual(true, mock_handle.HandleParam("check", HttpRequestHandlers.SingleCheckBox));
            
            List<string> l = new List<string>();
            l.Add("1");
            l.Add("2");
            l.Add("3");
            Assert.AreEqual(l, mock_handle.HandleParam("list", HttpRequestHandlers.CommaList));
        }
    }
}
