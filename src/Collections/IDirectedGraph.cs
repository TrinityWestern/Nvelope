
namespace Nvelope.Collections
{
    using System;
    using Nvelope;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

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

        public static IEnumerable<IDirectedGraph<T>> Traverse<T>(
            this IDirectedGraph<T> graph,
            TreeTraversal mode = TreeTraversal.PreOrder)
        {
            if (mode == TreeTraversal.PreOrder) {
                yield return graph;
                foreach (var child in graph.Children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }
            } else if (mode == TreeTraversal.InOrder) {
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
            } else if (mode == TreeTraversal.PostOrder) {
                foreach (var child in graph.Children) {
                    foreach (var item in child.Traverse(mode)) {
                        yield return item;
                    }
                }

                yield return graph;
            } else if (mode == TreeTraversal.LevelOrder) {
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
