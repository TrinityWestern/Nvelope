using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nvelope;
using System.Web;

namespace Nvelope.Tests
{
    [TestFixture, Obsolete("this should be moved to chocolate")]
    public class HtmlExtensionTests
    {
        [Test]
        public void AutoLink()
        {
            var examples = new Dictionary<string, string> {
                {"just a plain text url http://example.org",
                    "just a plain text url <a href='http://example.org'>http://example.org</a>"},
                {"a short url: http://ex#",
                    "a short url: <a href='http://ex#'>http://ex#</a>"},
                {"too short: ftp://f",
                    "too short: ftp://f"},
                {"too short with fragment: ftp://f#",
                    "too short with fragment: ftp://f#"},
                {"http://example.org.",
                    "<a href='http://example.org'>http://example.org</a>."},
                {"http://example.org?",
                    "<a href='http://example.org'>http://example.org</a>?"},
                {"http://example.org;",
                    "<a href='http://example.org'>http://example.org</a>;"},
                {"http://example.org,",
                    "<a href='http://example.org'>http://example.org</a>,"},
                {"&lt;http://example.org.&gt;",
                    "&lt;<a href='http://example.org.'>http://example.org.</a>&gt;"},
                {"(http://example.org)",
                    "(<a href='http://example.org'>http://example.org</a>)"},
                {"http://example.org: this website pwns",
                    "<a href='http://example.org'>http://example.org</a>: this website pwns"},
                {"",
                    ""}
            };
            foreach (var kv in examples) {
                Assert.AreEqual(kv.Value, HtmlExtensions.AutoLink(kv.Key));
            }
        }
    }
}
