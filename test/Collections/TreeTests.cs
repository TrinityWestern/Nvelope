using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope;
using Nvelope.Collections;

namespace Nvelope.Tests.Collections
{
    [TestFixture]
    public class TreeTests
    {
        protected DagTree<int> GrowTree()
        {
            var tree = new DagTree<int>(5);
            var branch = new DagTree<int>(3);
            branch.AppendChild(new DagTree<int>(1));
            branch.AppendChild(new DagTree<int>(2));
            tree.AppendChild(branch);
            tree.AppendChild(new DagTree<int>(7));
            return tree;
        }

        [Test]
        public void Contains()
        {
            var tree = GrowTree();
            Assert.IsTrue(tree.Contains(5));
            Assert.IsTrue(tree.Contains(3));
            Assert.IsTrue(tree.Contains(1));
            Assert.IsTrue(tree.Contains(2));
            Assert.IsTrue(tree.Contains(7));
        }

        [Test]
        public void Iteration()
        {
            var tree = GrowTree();
            Assert.AreEqual("(5,3,1,2,7)",
                tree.Select(n => n.Value).Print());
            Assert.AreEqual("(1,3,2,5,7)",
                tree.Traverse(TreeTraversal.InOrder)
                .Select(n => n.Value).Print());
            Assert.AreEqual("(1,2,3,7,5)",
                tree.Traverse(TreeTraversal.PostOrder)
                .Select(n => n.Value).Print());
            Assert.AreEqual("(5,3,7,1,2)",
                tree.Traverse(TreeTraversal.LevelOrder)
                .Select(n => n.Value).Print());
        }
        [Test]
        public void Formatting()
        {
            string format = "5\n  3\n    1\n    2\n  7\n";
            format = format.Replace("\n", Environment.NewLine);
            Assert.AreEqual(format, GrowTree().ToString());
        }

        [Test]
        public void Restructuring()
        {
            var tree = GrowTree();
            var node = tree.Children.First();
            tree.Promote(node);
            Assert.IsNull(node.Parent);
            Assert.AreEqual("(5,7)",
                tree.Select(n => n.Value).Print());
            Assert.AreEqual("(3,1,2,5,7)",
                node.Select(n => n.Value).Print());

        }

    }
}
