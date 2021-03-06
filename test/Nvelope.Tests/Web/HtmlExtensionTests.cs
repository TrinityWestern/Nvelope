﻿namespace Nvelope.Tests.Web
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Nvelope.Web;

    [TestFixture]
    class HtmlExtensionTests
    {
        [Test]
        public void HtmlFormat()
        {
            // list of expected html, format, and a single escaped paramenter
            var Examples = new List<Tuple<string, string, string>>
            {
                Tuple.Create("foo bar", "foo {0}", "bar"),
                Tuple.Create("<p>&lt;&gt;&quot;</p>", "<p>{0}</p>", "<>\""),
                Tuple.Create("This is Mr. O&#39;brian.", "This is {0}.", "Mr. O'brian"),
                Tuple.Create("<h1>&#39;Ali &amp; Ch&#238;&#39; &lt;0&gt;</h1>", "<h1>{0}</h1>", "'Ali & Chî' <0>"),
            };

            foreach (var ex in Examples)
            {
                Assert.AreEqual(ex.Item1, ex.Item2.HtmlFormat(ex.Item3).ToHtmlString());
            }
        }

        [Test]
        public void HtmlFormatFormats()
        {
            Assert.AreEqual("Amount: " + (10m).ToString("c"), "{0}: {1:c}".HtmlFormat("Amount", 10m).ToHtmlString());
        }

        [Test]
        public void HtmlFormatExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => "foo{0}bar".HtmlFormat(null));
            Assert.Throws<ArgumentNullException>(() => string.Format("foo{0}bar", null));
        }
        [Test]
        public void boom()
        {
            Assert.AreEqual("eer", "eer".HtmlFormat(123).ToHtmlString());
            //Index (zero based) must be greater than or equal to zero and less than the size of the argument list.
            Assert.Throws<FormatException>(() => "e{1}er".HtmlFormat(123).ToHtmlString());
            Assert.Throws<FormatException>(() => "e{0}e{1}r".HtmlFormat(123).ToHtmlString());
            Assert.AreEqual("ee123r", "ee{1}r".HtmlFormat(12, 123).ToHtmlString());
        }
    }
}
