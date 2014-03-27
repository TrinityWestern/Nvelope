using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Web;

namespace Nvelope.Tests.Web
{
    [TestFixture]
    class UriExtensionsTests
    {
        const string uriPlain = "http://test.com";
        const string uriSubDomain = "http://sub.test.com";
        const string uriSecure = "https://test.com";
        const string uriScript = "http://test.com/script.php";
        const string uriFriendly = "http://test.com/resource";
        const string uriParam = "http://test.com?foo=1";
        const string uriParams = "http://test.com?foo=1&bar=2";
        const string uriNullParam = "http://test.com?foo";
        const string uriFragment = "http://test.com#foo";

        [TestCase(uriPlain,     Result = uriPlain)]
        [TestCase(uriSubDomain, Result = uriSubDomain)]
        [TestCase(uriSecure,    Result = uriSecure)]
        [TestCase(uriScript,    Result = uriScript)]
        [TestCase(uriFriendly,  Result = uriFriendly)]
        [TestCase(uriParam,     Result = uriParam)]
        [TestCase(uriParams,    Result = uriParams)]
        [TestCase(uriNullParam, Result = uriNullParam)]
        [TestCase(uriFragment,  Result = uriFragment)]
        public string NoSlash(string uri)
        {
            return new Uri(uri).ToStringWithoutSlash();
        }

        [TestCase(uriPlain,     Result = uriPlain + "?test=3")]
        [TestCase(uriSubDomain, Result = uriSubDomain + "?test=3")]
        [TestCase(uriSecure,    Result = uriSecure + "?test=3")]
        [TestCase(uriScript,    Result = uriScript + "?test=3")]
        [TestCase(uriFriendly,  Result = uriFriendly + "?test=3")]
        [TestCase(uriParam,     Result = uriParam + "&test=3")]
        [TestCase(uriParams,    Result = uriParams + "&test=3")]
        [TestCase(uriNullParam, Result = uriNullParam + "&test=3")]
        [TestCase(uriFragment,  Result = "http://test.com?test=3#foo")]
        public string AddParam(string uri)
        {
            return new Uri(uri).AddParameter("test", "3").ToStringWithoutSlash();
        }
    }
}
