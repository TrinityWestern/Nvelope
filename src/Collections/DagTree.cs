using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

namespace Nvelope.Collections
{
    
    /// <summary>
    /// Represents a Node in a tree
    /// 
    /// Note: This is actually a connected DAG in mathematical terms, not a tree.
    /// </summary>
    [Serializable]
    public class DagTree<T> : IComparable, IEnumerable, IEnumerable<DagTree<T>>, ITreeable<T> where T : IComparable
    {
        /// <summary>
        /// The data associated with this node
        /// </summary>
        public T Value;
        /// <summary>
        /// Create new treenode
        /// </summary>
        /// <param name="data"></param>
        public DagTree(T data)
        {            
            Value = data;
            _parent = null;
            _children = new List<DagTree<T>>();
        }

        protected DagTree<T> _parent;
        protected List<DagTree<T>> _children;

        /// <summary>
        /// The parent node (null if this is the root node)
        /// </summary>
        public DagTree<T> Parent
        {
            get { return _parent; }
        }
        /// <summary>
        /// The children of this node
        /// </summary>
        public IEnumerable<DagTree<T>> Children
        {
            get { return _children; }
        }

        public IEnumerable<ITreeable<T>> GetChildren()
        {
            return _children;
        }
        public T Item {
            get { return this.Value; }
        }

        /// <summary>
        /// Add a new child node.
        /// </summary>
        /// <param name="newChild"></param>
        public DagTree<T> AppendChild(DagTree<T> node)
        {
            var testnode = this;
            while (testnode != null) {
                if (node.Eq(testnode)) {
                    throw new ArgumentException(
                        "Adding a node that is already in the tree, this will create a cycle");
                } else {
                    testnode = testnode.Parent;
                }
            }

            node._parent = this;
            this._children.Add(node);
            return node;
        }

        /// <summary>
        /// Remove a child node.
        /// </summary>
        /// <param name="node"></param>
        public DagTree<T> RemoveChild(DagTree<T> node)
        {
            this._children.Remove(node);
            node._parent = null;
            return node;
        }

        /// <summary>
        /// Checks to see if a decendent has the following value
        /// </summary>
        [Obsolete("I'm not sure this is a good thing to use, just do the select yourself")]
        public bool Contains(T value)
        {
            return this.Select(n => n.Value).Contains(value);
        }

        /// <summary>
        /// Bring a subtree to the top of the tree
        /// 
        /// Subordiates the rest of the tree to the root node of the subtree.
        /// </summary>
        /// <param name="subtree"></param>
        public void Promote(DagTree<T> node)
        {
            if (node == this) return;

            node.Parent.RemoveChild(node);
            node.AppendChild(this);
        }
        /// <summary>
        /// prints a nested list
        /// </summary>
        public override string ToString() {
            return this.ToString("");
        }
        
        public IEnumerable<DagTree<T>> Traverse(
            TreeTraversal mode = TreeTraversal.PreOrder)
        {
            if (mode == TreeTraversal.PreOrder) {
                yield return this;
                foreach (var child in this.Children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }

            } else if (mode == TreeTraversal.InOrder) {

                var children = this.Children.Copy();

                if (children.Any()) {
                    var first = children.Unshift();
                    foreach (var item in first.Traverse(mode)) {
                        yield return item;
                    }
                }

                yield return this;

                foreach (var child in children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }

            } else if (mode == TreeTraversal.PostOrder) {

                foreach (var child in this.Children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }
                yield return this;

            } else if (mode == TreeTraversal.LevelOrder) {
                yield return this;

                var level = this.Children;
                var nextlevel = new List<DagTree<T>>();

                while (level.Any()) {
                    foreach (var item in level) {
                        yield return item;
                        nextlevel.AddRange(item.Children);
                    }
                    level = nextlevel.Copy();
                    nextlevel.Clear();
                }
            } else {
                throw new ArgumentException("Unknown traversal mode");
            }
        }

        IEnumerator<DagTree<T>> IEnumerable<DagTree<T>>.GetEnumerator()
        {
            foreach (var item in this.Traverse()) yield return item;
        }
        /// <summary>
        /// Silly method required for the old .NET interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (object item in this) yield return item;
        }

#region IComparable Members
        /// <summary>
        /// Implements CompareTo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            if (!(obj is DagTree<T>))
                return 1;
            var other = obj as DagTree<T>;
            return this.Value.CompareTo(other.Value);
        }

#endregion
#region privates
        protected string ToString(string prefix)
        {
            string res = prefix + Value.ToString() + "\n";
            foreach (DagTree<T> child in this.Children)
                res += child.ToString(prefix + "  ");

            return res;
        }
#endregion
    }

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
}
