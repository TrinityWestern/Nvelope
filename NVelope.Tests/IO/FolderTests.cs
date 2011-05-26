using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.IO;

namespace Nvelope.Tests.IO
{
    [TestFixture]
    public class FolderTests
    {
        [Test]
        public void ComposePath()
        {
            Assert.AreEqual("C:\\Abc\\De\\fg", Folder.ComposePath("C:\\Abc\\", "\\De\\", "\\fg\\"));
            Assert.AreEqual("C:\\Abc\\De\\fg", Folder.ComposePath("C:\\Abc\\", "\\De\\", "\\fg"));
            Assert.AreEqual("C:\\Abc\\De\\fg", Folder.ComposePath("C:\\Abc\\", "\\De\\", "fg"));
            Assert.AreEqual("C:\\Abc\\De\\fg", Folder.ComposePath("C:\\Abc\\", "\\De", "fg"));
            Assert.AreEqual("C:\\Abc\\De\\fg", Folder.ComposePath("C:\\Abc\\", "De", "fg"));
            Assert.AreEqual("C:\\Abc\\De\\fg", Folder.ComposePath("C:\\Abc", "De", "fg"));

            Assert.AreEqual("C:\\Abc\\De\\fg.exe", Folder.ComposePath("C:\\Abc\\", "\\De\\", "\\fg.exe"));
            Assert.AreEqual("C:\\Abc\\De\\fg.exe", Folder.ComposePath("C:\\Abc\\", "\\De\\", "fg.exe"));
            Assert.AreEqual("C:\\Abc\\De\\fg.exe", Folder.ComposePath("C:\\Abc\\", "\\De", "fg.exe"));
            Assert.AreEqual("C:\\Abc\\De\\fg.exe", Folder.ComposePath("C:\\Abc\\", "De", "fg.exe"));
            Assert.AreEqual("C:\\Abc\\De\\fg.exe", Folder.ComposePath("C:\\Abc", "De", "fg.exe"));

            Assert.AreEqual("\\Abc\\De\\fg.exe", Folder.ComposePath("\\Abc\\", "\\De\\", "\\fg.exe"));
            Assert.AreEqual("\\Abc\\De\\fg.exe", Folder.ComposePath("\\Abc\\", "\\De\\", "fg.exe"));
            Assert.AreEqual("\\Abc\\De\\fg.exe", Folder.ComposePath("\\Abc\\", "\\De", "fg.exe"));
            Assert.AreEqual("\\Abc\\De\\fg.exe", Folder.ComposePath("\\Abc\\", "De", "fg.exe"));
            Assert.AreEqual("\\Abc\\De\\fg.exe", Folder.ComposePath("\\Abc", "De", "fg.exe"));
            
            Assert.AreEqual("Abc\\De\\fg.exe", Folder.ComposePath("Abc\\", "\\De\\", "\\fg.exe"));
            Assert.AreEqual("Abc\\De\\fg.exe", Folder.ComposePath("Abc\\", "\\De\\", "fg.exe"));
            Assert.AreEqual("Abc\\De\\fg.exe", Folder.ComposePath("Abc\\", "\\De", "fg.exe"));
            Assert.AreEqual("Abc\\De\\fg.exe", Folder.ComposePath("Abc\\", "De", "fg.exe"));
            Assert.AreEqual("Abc\\De\\fg.exe", Folder.ComposePath("Abc", "De", "fg.exe"));

            Assert.AreEqual("Abc", Folder.ComposePath("Abc", ""));
            Assert.AreEqual("Abc", Folder.ComposePath("Abc"));

            Assert.AreEqual("/packages/support/helpdesk/ajax_ticketlist.ashx", Folder.ComposePath("/packages", "support/helpdesk/ajax_ticketlist.ashx"));
        }

        [Test]
        public void ComposePath_DifferentSeperators()
        {
            Assert.AreEqual("svn://server.twu.ca/foo/bar", Folder.ComposePath("svn://server.twu.ca", "foo\\bar"));
        }

        [Test]
        public void Directory()
        {
            Assert.AreEqual("\\a\\b\\c\\", Folder.Directory("\\a\\b\\c\\d.txt"));
            Assert.AreEqual("", Folder.Directory("a.txt"));
            Assert.AreEqual("dir", Folder.Directory("dir"));

            Assert.AreEqual("c:\\.hudson\\a\\b\\c\\", Folder.Directory("c:\\.hudson\\a\\b\\c\\abc.txt"));
        }

        [Test]
        public void File()
        {
            Assert.AreEqual("a.txt", Folder.File("\\a\\b\\c\\a.txt"));
            Assert.AreEqual("a.txt", Folder.File("a.txt"));
            Assert.AreEqual("", Folder.File("dir"));
            Assert.AreEqual("abc.txt", Folder.File("c:\\.hudson\\a\\b\\c\\abc.txt"));
        }

        [Test]
        public void Extension()
        {
            Assert.AreEqual("txt", Folder.Extension("\\a\\b\\c\\a.txt"));
            Assert.AreEqual("user", Folder.Extension("\\a\\b\\a.csproj.user"));
            Assert.AreEqual("txt", Folder.Extension("c:\\.hudson\\a\\b\\c\\abc.txt"));
            Assert.AreEqual("", Folder.Extension("\\a\\b\\foosums"));
            Assert.AreEqual("", Folder.Extension("foosums"));
        }

        [Test]
        public void FolderParts()
        {
            Assert.AreEqual("(a,b,c,d)", Folder.FolderParts("a\\b\\c\\d\\e.txt").Print());
            Assert.AreEqual("(a,b,c,d)", Folder.FolderParts("a/b/c/d/e.txt").Print());

            Assert.AreEqual("(a,b,c)", Folder.FolderParts("a\\b\\c\\").Print());
            Assert.AreEqual("(a,b,c)", Folder.FolderParts("a/b/c/").Print());
            Assert.AreEqual("(C:,a,b)", Folder.FolderParts("C:\\a\\b\\").Print());
            Assert.AreEqual("(aqueduct)", Folder.FolderParts("/aqueduct").Print());
            Assert.AreEqual("(aqueduct)", Folder.FolderParts("/aqueduct/").Print());
            Assert.AreEqual("(aqueduct,helpdesk)", Folder.FolderParts("/aqueduct/helpdesk").Print());
        }

        [Test]
        public void FileBaseName()
        {
            Assert.AreEqual("foosums", Folder.FileBaseName("foosums.bar"));
            Assert.AreEqual("foosums", Folder.FileBaseName("\\a\\b\\c\\foosums.bar"));
        }

        [Test]
        public void ResolvePath()
        {
            Assert.AreEqual("c:\\a\\b\\c", Folder.ResolvePath("c:\\a\\b", "c"));
            Assert.AreEqual("c:\\a\\b\\c", Folder.ResolvePath("c:\\a", "b\\c\\"));
            // Handle . (current directory)
            Assert.AreEqual("c:\\a\\b\\c", Folder.ResolvePath("c:\\a", ".\\b\\c"));
            // Handle .. (parent dir)
            Assert.AreEqual("c:\\a\\y\\z", Folder.ResolvePath("c:\\a\\b\\c", "..\\..\\y\\z"));

            Assert.AreEqual(        "C:\\gitRepo\\aqueduct-dev\\Build\\Aqueduct.Lib.Build.Tests\\trunk\\bin\\Debug", 
                Folder.ResolvePath("C:\\gitRepo\\aqueduct-dev\\Build\\Aqueduct.Lib.Build.Tests\\trunk\\bin\\Debug",
                                    "C:\\gitRepo\\aqueduct-dev\\Build\\Aqueduct.Lib.Build.Tests\\trunk\\bin\\Debug"));

            Assert.AreEqual(        "C:\\gitRepo\\aqueduct\\Aqueduct.Lib.Database\\trunk",
                Folder.ResolvePath("C:\\gitRepo\\aqueduct-dev\\Build\\hudsonJob\\trunk\\bin\\Debug",
                                    "C:\\gitRepo\\aqueduct\\Aqueduct.Lib.Database\\trunk"));


            Assert.AreEqual("c:\\a\\b\\c.txt", Folder.ResolvePath("c:\\a", "b\\c.txt"));

            // It should work for relative base paths as well
            Assert.AreEqual("..\\a\\bb\\c", Folder.ResolvePath("..\\a\\b", "..\\bb\\c"));

        }

        [Test]
        public void IsRoot()
        {
            Assert.IsFalse(Folder.IsRoot("c:\\a\\b"));
            Assert.IsFalse(Folder.IsRoot("c:\\a"));
            Assert.IsTrue(Folder.IsRoot("c:"));
            Assert.IsTrue(Folder.IsRoot("C:\\"));

            Assert.IsTrue(Folder.IsRoot("\\a"));
            Assert.IsFalse(Folder.IsRoot("\\a\\b"));
        }

        [Test]
        public void Parent()
        {
            Assert.AreEqual("c:\\a", Folder.Parent("c:\\a\\b"));
            Assert.AreEqual("", Folder.Parent("c:\\"));

            Assert.AreEqual("", Folder.Parent("\\a"));
        }

        [Test]
        public void GetSeperator()
        {
            Assert.AreEqual("\\", Folder.GetSeperator("c:\\a", ""));
            Assert.AreEqual("\\", Folder.GetSeperator("hello.txt", "\\"));
            Assert.AreEqual("\\", Folder.GetSeperator("c:\\a//b", ""));
            
            Assert.AreEqual("/", Folder.GetSeperator("//a//b", ""));
            Assert.AreEqual("/", Folder.GetSeperator("svn://a//b", ""));

            Assert.AreEqual("/", Folder.GetSeperator(new string[] { "svn://server.twu.ca", "c:\\a\\b" }, ""));
        }

        [Test]
        public void UseSeperator()
        {
            Assert.AreEqual("c:\\a\\b\\c", Folder.UseSeperator("c:\\a\\b\\c", "\\"));
            Assert.AreEqual("c:\\a\\b\\c", Folder.UseSeperator("c:\\a/b\\c", "\\"));
            Assert.AreEqual("svn://a/b/c", Folder.UseSeperator("svn://a\\b\\c", "/"));
        }

        //[Test]
        //public void RelativePath()
        //{
        //    // We should offset to \a\b\cc, then from there go to \d
        //    Assert.AreEqual("C:\\a\\b\\cc\\d", Folder.RelativePath("C:\\a\\b\\c", "..\\cc", "\\d"));
        //}
    }
}
