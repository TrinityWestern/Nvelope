//-----------------------------------------------------------------------
// <copyright file="DirectedAcyclicGraph.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// <para>Represents a Node in a tree</para>
    /// <para>This is actually a connected DAG in mathematical terms, similar to a tree.</para>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Graph suffix is good enough")]
    [Serializable]
    public class DirectedAcyclicGraph<T> : IDirectedGraph<T>
    {
        private DirectedAcyclicGraph<T> parent;
        private List<DirectedAcyclicGraph<T>> children;

        /// <summary>
        /// Create new treenode
        /// </summary>
        /// <param name="data">The data.</param>
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
        /// <value>
        /// The parent.
        /// </value>
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

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T Item
        {
            get { return this.Value; }
        }

        /// <summary>
        /// Add a new child node.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Adding a node that is already in the tree, this will create a cycle</exception>
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
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        public IDirectedGraph<T> RemoveChild(IDirectedGraph<T> graph)
        {
            this.children.Remove((DirectedAcyclicGraph<T>)graph);
            graph.Parent = null;
            return graph;
        }

        /// <summary>
        /// Checks to see if a decendent has the following value
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            return this.Traverse().Select(n => n.Value).Contains(value);
        }

        /// <summary>
        /// Bring a subtree to the top of the tree
        /// Subordinates the rest of the tree to the root node of the subtree.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Promote(IDirectedGraph<T> node)
        {
            if (node == this) return;

            node.Parent.RemoveChild(node);
            node.AppendChild(this);
        }

        /// <summary>
        /// prints a nested list
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            return this.ToString(string.Empty);
        }

        #region privates

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
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
