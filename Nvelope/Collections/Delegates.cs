using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nvelope.Collections
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="runningTotal"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    //public delegate U SummingFunction<T,U>(U runningTotal, T item);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    //public delegate T Function<T>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    //public delegate U Conversion<T,U>(T item);

    /// <summary>
    /// A delegate that performs some function on an item of type T
    /// and returns a result of type U
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    //public delegate U CollectionFunctionDelegate<T, U>(T input);

    /// <summary>
    /// A delegate that takes two items and turns them into one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    //public delegate T CombinationFunctionDelegate<T>(T a, T b);

    /// <summary>
    /// A delegate that takes two items and turns them into one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    //public delegate T CombinationFunctionDelegate<T,U,V>(U a, V b);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void CollectionModificationEventHandler<T>(object sender, CollectionModificationEventArgs<T> e);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void CollectionModificationEventHandler<TKey, TValue>(object sender, CollectionModificationEventArgs<TKey, TValue> e);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void CollectionAccessEventHandler<T>(object sender, CollectionAccessEventArgs<T> e);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    //public delegate Dictionary<TKey, TValue> LookupDictionaryRetrieveDelegate<TKey, TValue>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    //public delegate bool LookupDictionaryTimingDelegate<TKey>(TKey key);

    /// <summary>
    /// Takes a T as an argument, and returns a string regarding it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <remarks>It is not necessary for this function to take care of child nodes - they will be handled automajically.
    /// You're welcome =)</remarks>
    //public delegate System.Web.UI.WebControls.TreeNode ParseDelegate<T>(TreeNode<T> data) where T : IComparable;
    /// <summary>
    /// A delegate that specifies how to fetch a single "next" item from a collection, based on an initial item.
    /// </summary>
    /// <example>Returning the next item in a list, given an initial node</example>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    //public delegate T SingleTraversalDelegate<T>(T input);
    /// <summary>
    /// A delegate that specifies how to fetch a group of "next" items from a collection, based on an initial item.
    /// </summary>
    /// <example>Returning the child nodes of a node in a tree</example>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public delegate IEnumerable<T> MultipleTraversalDelegate<T>(T input);

    /// <summary>
    /// A delegate describing how to convert a T to an XmlElement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementProvider"></param>
    /// <param name="dataItem"></param>
    /// <returns></returns>
    //public delegate XmlElement ConvertToXmlElementDelegate<T>(XmlDocument elementProvider, T dataItem) where T : IComparable;
    
    
}
