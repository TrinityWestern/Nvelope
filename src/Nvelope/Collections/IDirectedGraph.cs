//-----------------------------------------------------------------------
// <copyright file="IDirectedGraph.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nvelope;

    public enum DirectedGraphTraversal
    {
        /// <summary>
        /// root, left subtree, right subtree
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
            Justification = "This is pre-order traversal, not preordering.")]
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
    /// This interface allows the implementing class to be used in various tree-based 
    /// data structures.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDirectedGraph<T>
    {
        IDirectedGraph<T> Parent { get; set; }

        /// <summary>
        /// This function returns an enumeration (array, list, whatever) of the children
        /// of the object.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This complication is necessary for graphs")]
        IEnumerable<IDirectedGraph<T>> Children { get; }

        /// <summary>
        /// Returns the item itself
        /// </summary>
        T Value { get; }

        IDirectedGraph<T> AppendChild(IDirectedGraph<T> graph);

        IDirectedGraph<T> RemoveChild(IDirectedGraph<T> graph);
    }

    public static class IDirectedGraphExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This complication is necessary for graphs")]
        public static IEnumerable<IDirectedGraph<T>> Traverse<T>(
            this IDirectedGraph<T> graph,
            DirectedGraphTraversal mode = DirectedGraphTraversal.PreOrder)
        {
            if (mode == DirectedGraphTraversal.PreOrder) {
                yield return graph;
                foreach (var child in graph.Children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }
            } else if (mode == DirectedGraphTraversal.InOrder) {
                var children = graph.Children.Copy();

                if (children.Any()) {
                    var first = children.Unshift();
                    foreach (var item in first.Traverse(mode)) {
                        yield return item;
                    }
                }

                yield return graph;

                foreach (var child in children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }
            } else if (mode == DirectedGraphTraversal.PostOrder) {
                foreach (var child in graph.Children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }

                yield return graph;
            } else if (mode == DirectedGraphTraversal.LevelOrder) {
                yield return graph;

                var level = graph.Children;
                var nextlevel = new List<IDirectedGraph<T>>();

                while (level.Any()) {
                    foreach (var item in level) {
                        yield return item;
                        nextlevel.AddRange(item.Children);
                    }

                    level = nextlevel.Copy();
                    nextlevel.Clear();
                }
            } else { throw new ArgumentException("Unknown traversal mode"); }
        }
    }
}
