using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml;

namespace Nvelope.Tests
{
    [TestFixture]
    public class XmlExtensionTests
    {
        [Test]
        public void SetVal()
        {
            XmlDocument doc = "<root><child><grandchild>GC11</grandchild><grandchild>GC12</grandchild></child><child>C2</child></root>"
                .ToXml();

            doc.SetVal("/root/child[2]", "secondChild");
            // Assumes Val is working properly
            Assert.AreEqual("secondChild", doc.Val("/root/child[2]"));

            doc.SetVals("/root/child/grandchild", "grandkid");
            // Assumes Vals is working properly
            Assert.AreEqual("(grandkid,grandkid)", doc.Vals("/root/child/grandchild").Print());            
        }

        [Test]
        public void SetVal_creates_nodes_in_existing_document()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");

            var path = "/configuration/appSettings/add[@key = 'MyReportServerUrl']/@value";
            doc.SetVal(path, "foosums");
            Assert.AreEqual("foosums", doc.Val(path));
        }

        [Test]
        public void SetValReturnsOriginalNode()
        {
            XmlDocument doc = "<root><child><grandchild>GC11</grandchild><grandchild>GC12</grandchild></child><child>C2</child></root>"
                .ToXml();
            var setRes = doc.SetVal("/root/child[2]", "secondChild");
            Assert.AreEqual(doc, setRes);
        }

        [Test]
        public void SetValCreatesNodes()
        {
            XmlDocument doc = "<root><child></child></root>".ToXml();

            doc.SetVal("/root/child/grandchild/greatgrandchild", "GGC");
            Assert.AreEqual("<root><child><grandchild><greatgrandchild>GGC</greatgrandchild></grandchild></child></root>",
                doc.InnerXml);

            doc = "<root><child></child><child></child></root>".ToXml();
            // Should only set one value, since it's SetVal, not SetVals
            doc.SetVal("/root/child/grandchild", "GC");
            Assert.AreEqual("<root><child><grandchild>GC</grandchild></child><child></child></root>",
                doc.InnerXml);

            doc = "<root><child></child><child></child></root>".ToXml();
            // Should throw an exception, since we're trying to set multiple nodes that don't exist,
            // so we've got no way of knowing which ones to create
            Assert.Throws(typeof(Exception), () => doc.SetVals("/root/child/grandchild", "GC"));
        }

        [Test]
        public void SetValCreatesNodesRelativeToAnotherNode()
        {
            XmlDocument doc = "<a><aa><aaa></aaa></aa></a>".ToXml();

            var node = doc.Node("/a/aa");
            node.SetVal("./bbb/bbbb", "foosums");

            Assert.AreEqual("<a><aa><aaa></aaa><bbb><bbbb>foosums</bbbb></bbb></aa></a>", doc.InnerXml);
        }

        [Test]
        public void Val()
        {
            XmlDocument doc = "<root><child><grandchild>GC11</grandchild><grandchild>GC12</grandchild></child><child>C2</child></root>"
                .ToXml();

            Assert.AreEqual("C2", doc.Val("/root/child[2]"));
            Assert.AreEqual("(GC11,GC12)", doc.Vals("/root/child/grandchild").Print());
        }

        [Test]
        public void Val_Part2()
        {
            // Test values
            var xml = "<Root><Node val=\"a\"/><Node val=\"b\"/></Root>".ToXml();
            Assert.AreEqual("(a,b)", xml.Vals("//Node/@val").Print());

            // Test inner texts
            xml = "<Root><Node>a</Node><Node>b</Node></Root>".ToXml();
            Assert.AreEqual("(a,b)", xml.Vals("//Node").Print());            
        }

        [Test]
        public void ValN()
        {
            var xml = "<Root><Node val=\"a\">true</Node></Root>".ToXml();
            Assert.AreEqual("a", xml.ValN("/Root/Node/@val"));
            Assert.AreEqual("", xml.ValN("/Root/foobar"));
            Assert.AreEqual(null, xml.ValN<bool?>("/Root/foobar"));
            Assert.AreEqual(true, xml.ValN<bool?>("/Root/Node"));
        }

        [Test]
        public void AddChild()
        {
            XmlDocument doc = "<root><child><grandchild>GC1</grandchild></child></root>".ToXml();
            doc.Node("/root/child").AddChild("grandchild", "GC2");
            Assert.AreEqual("<root><child><grandchild>GC1</grandchild><grandchild>GC2</grandchild></child></root>",
                doc.InnerXml);
        }

        [Test]
        public void AddChildReturnsNewNode()
        {
            XmlDocument doc = "<root><child><grandchild>GC1</grandchild></child></root>".ToXml();
            // AddChild should return the original node, so that things can be chained
            var addRes = doc.Node("/root/child").AddChild("grandchild", "GC2");
            Assert.AreEqual(addRes, doc.Node("/root/child/grandchild[2]"));
        }

        [Test]
        public void NamespaceXpath()
        {
            Assert.AreEqual("//x:Project", "//Project".NamespaceXPath("x"));
            Assert.AreEqual("/x:Project/x:PropertyGroup", "/Project/PropertyGroup".NamespaceXPath("x"));
            Assert.AreEqual("x:Project", "Project".NamespaceXPath("x"));

            Assert.AreEqual("//x:Project/@id", "//Project/@id".NamespaceXPath("x"));
            Assert.AreEqual("/x:Project[x:ItemGroup]", "/Project[ItemGroup]".NamespaceXPath("x"));
            Assert.AreEqual("/x:Project[@id = '7']", "/Project[@id = '7']".NamespaceXPath("x"));
            Assert.AreEqual("/x:Project[@id = '7']/x:Compile/@include", "/Project[@id = '7']/Compile/@include".NamespaceXPath("x"));

            Assert.AreEqual("//x:Project[1]/x:Compile", "//Project[1]/Compile".NamespaceXPath("x"));

            Assert.AreEqual("@id", "@id".NamespaceXPath("x"));

            Assert.AreEqual("./x:Name", "./Name".NamespaceXPath("x"));            
            Assert.AreEqual(".//x:Name", ".//Name".NamespaceXPath("x"));
            Assert.AreEqual("@Name", "@Name".NamespaceXPath("x"));
            Assert.AreEqual("./x:Name/@id", "./Name/@id".NamespaceXPath("x"));

        }

        [Test]
        public void IsAttribute()
        {
            Assert.IsTrue("@val".IsAttribute());
            Assert.IsTrue("/@val".IsAttribute());
            Assert.IsTrue("/node/@val".IsAttribute());

            Assert.IsFalse("node".IsAttribute());
            Assert.IsFalse("/x:node/x:other".IsAttribute());
            Assert.IsFalse("/x:add[@key = 'foobar']".IsAttribute());

            Assert.IsTrue("/x:add[@key = 'foobar']/@value".IsAttribute());
        }

    }
}
