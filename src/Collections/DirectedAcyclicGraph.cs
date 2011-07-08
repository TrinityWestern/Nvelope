//-----------------------------------------------------------------------
// <copyright file="DirectedAcyclicGraph.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Graph suffix is good enough")]
    public enum TreeTraversal
    {
        /// <summary>
        /// root, left subtree, right subtree
        /// </summary>
        PreOrder,

        /// <summary>
        /// left subtree, root, right subtree
        /// </summary>
        InOrder,

        /// <summary>
        /// left subtree, right subtree, root
        /// </summary>
        PostOrder,

        /// <summary>
        /// self, then children, then grandchildren, great grandchildren, etc
        /// </summary>
        LevelOrder
    }
    
    /// <summary>
    /// <para>Represents a Node in a tree</para>
    /// </para>This is actually a connected DAG in mathematical terms, similar to a tree.</para>
    /// </summary>
    [Serializable]
    public class DirectedAcyclicGraph<T> : IDirectedGraph<T>
    {
        private DirectedAcyclicGraph<T> parent;
        private List<DirectedAcyclicGraph<T>> children;

        /// <summary>
        /// Create new treenode
        /// </summary>
        /// <param name="data"></param>
        public DirectedAcyclicGraph(T data)
        {
            this.Value = data;
            this.parent = null;
            this.children = new List<DirectedAcyclicGraph<T>>();
        }

        /// <summary>
        /// The data associated with this node
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The parent node (null if this is the root node)
        /// </summary>
        public IDirectedGraph<T> Parent
        {
            get { return this.parent; }
            set { this.parent = (DirectedAcyclicGraph<T>)value; }
        }

        /// <summary>
        /// The children of this node
        /// </summary>
        public IEnumerable<IDirectedGraph<T>> Children
        {
            get { return this.children; }
        }

        public T Item
        {
            get { return this.Value; }
        }

        /// <summary>
        /// Add a new child node.
        /// </summary>
        /// <param name="newChild"></param>
        public IDirectedGraph<T> AppendChild(IDirectedGraph<T> graph)
        {
            IDirectedGraph<T> testnode = this;
            while (testnode != null) {
                if (graph.Eq(testnode))
                {
                    throw new ArgumentException(
                        "Adding a node that is already in the tree, this will create a cycle");
                } else {
                    testnode = testnode.Parent;
                }
            }

            graph.Parent = this;
            this.children.Add((DirectedAcyclicGraph<T>)graph);
            return graph;
        }

        /// <summary>
        /// Remove a child node.
        /// </summary>
        /// <param name="node"></param>
        public IDirectedGraph<T> RemoveChild(IDirectedGraph<T> graph)
        {
            this.children.Remove((DirectedAcyclicGraph<T>)graph);
            graph.Parent = null;
            return graph;
        }

        /// <summary>
        /// Checks to see if a decendent has the following value
        /// </summary>
        [Obsolete("I'm not sure this is a good thing to use, just do the select yourself")]
        public bool Contains(T value)
        {
            return this.Traverse().Select(n => n.Value).Contains(value);
        }

        /// <summary>
        /// Bring a subtree to the top of the tree
        /// 
        /// Subordiates the rest of the tree to the root node of the subtree.
        /// </summary>
        /// <param name="subtree"></param>
        public void Promote(IDirectedGraph<T> node)
        {
            if (node == this) return;

            node.Parent.RemoveChild(node);
            node.AppendChild(this);
        }

        /// <summary>
        /// prints a nested list
        /// </summary>
        public override string ToString() {
            return this.ToString(string.Empty);
        }

#region privates
        protected string ToString(string prefix)
        {
            string res = prefix + this.Value.ToString() + Environment.NewLine;
            foreach (DirectedAcyclicGraph<T> child in this.Children) {
                res += child.ToString(prefix + "  ");
            }
            return res;
        }
#endregion
    }
}
