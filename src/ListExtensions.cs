using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class ListExtensions
    {
        /// <summary>
        /// Make a shallow copy of a list/enumeration
        /// 
        /// This makes a copy of the list itself, but all the contents are
        /// still reverences to the same objects
        /// </summary>
        public static List<T> Copy<T>(this IEnumerable<T> list)
        {
            var copy = new List<T>();
            copy.AddRange(list);
            return copy;
        }

        /// <summary>
        /// Adds an item on the end of the list
        /// 
        /// Just here for completion with the push/pop/shift/unshift methods
        /// </summary>
        public static void Push<T>(this List<T> list, T item)
        {
            list.Add(item);
        }
        /// <summary>
        /// Takes an item off the end of the list and returns it
        /// </summary>
        public static T Pop<T>(this List<T> list)
        {
            var last = list.Last();
            list.Remove(last);
            return last;
        }
        /// <summary>
        /// Adds and item ot the front of a list
        /// </summary>
        public static void Shift<T>(this List<T> list, T item)
        {
            list.Insert(0, item);
        }
        /// <summary>
        /// Takes an item off the front of the list and returns it
        /// </summary>
        public static T Unshift<T>(this List<T> list)
        {
            var first = list.First();
            list.Remove(first);
            return first;
        }

        public static string ToSeperatedList<T>(this IEnumerable<T> list, string seperator)
        {
            StringBuilder res = new StringBuilder();

            foreach (var item in list)
                res.Append(item).Append(seperator);

            // Remove trailing seperator
            if(res.Length >= seperator.Length)
                res.Remove(res.Length - seperator.Length, seperator.Length);

            return res.ToString();
        }

        public static List<T> Paginate<T>(this IEnumerable<T> list, int itemsPerPage, int page)
        {
            if (page > 0)
            {
                list = list.Skip((page - 1) * itemsPerPage);
            }
            return list.Take(itemsPerPage).ToList();
        }

        /// <summary>
        /// All the items in the list except the ones specified
        /// </summary>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Except(items as IEnumerable<T>);
        }

        /// <summary>
        /// Add one list to the end of another
        /// </summary>
        public static IEnumerable<T> And<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            foreach (T t in source)
                yield return t;
            foreach (T t in other)
                yield return t;
        }

        /// <summary>
        /// Returns the first list. If it's empty, return the alternate list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IEnumerable<T> Or<T>(this IEnumerable<T> source, IEnumerable<T> alternate)
        {
            if (source.Any())
                return source;
            else
                return alternate;
        }

        /// <summary>
        /// Returns everything after first. If there is nothing else, returns an empty IEnumerable T
        /// </summary>
        public static IEnumerable<T> Rest<T>(this IEnumerable<T> source)
        {
            if (source.Any())
                return source.Skip(1);
            else
                return new List<T>();
        }
        
        #region Numerical stuff
        /// <summary>
        /// Returns the first item in the list, if there are any. If the list is empty,
        /// return the alternate item
        /// </summary>
        public static T FirstOr<T>(this IEnumerable<T> list, T alternate)
        {
            if (list.Any())
                return list.First();
            else
                return alternate;
        }

        /// <summary>
        /// Returns the first item in the list that passes the predicate, or alternate if none do
        /// </summary>
        public static T FirstOr<T>(this IEnumerable<T> list, Func<T, bool> predicate, T alternate)
        {
            return list.Where(predicate).FirstOr(alternate);
        }

        public static T LastOr<T>(this IEnumerable<T> list, T alternate)
        {
            if (list.Any())
                return list.Last();
            else
                return alternate;
        }

        public static T Second<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(1);
        }

        public static T Third<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(2);
        }

        public static T Fourth<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(3);
        }

        public static T Fifth<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(4);
        }

        public static T ElementOr<T>(this IEnumerable<T> list, int index, T alternate)
        {
            if (index < int.MaxValue && list.AtLeast(index + 1))
                return list.ElementAt(index);
            
            return alternate;
        }

        public static T SecondOr<T>(this IEnumerable<T> list, T alternate)
        {
            return ElementOr(list, 1, alternate);
        }

        public static T ThirdOr<T>(this IEnumerable<T> list, T alternate)
        {
            return ElementOr(list, 2, alternate);
        }

        public static T FourthOr<T>(this IEnumerable<T> list, T alternate)
        {
            return ElementOr(list, 3, alternate);
        }

        public static T FifthOr<T>(this IEnumerable<T> list, T alternate)
        {
            return ElementOr(list, 4, alternate);
        }

        #endregion

        /// <summary>
        /// Takes two sequences and combines them element-by-element into a third
        /// </summary>
        public static IEnumerable<Tuple<TThis, TOther>> Zip<TThis, TOther>
            (this IEnumerable<TThis> source, IEnumerable<TOther> other)
        {
            using (var sourceEnum = source.GetEnumerator())
            {
                using (var otherEnum = other.GetEnumerator())
                {
                    while (sourceEnum.MoveNext() && otherEnum.MoveNext())
                        yield return Tuple.Create(sourceEnum.Current, otherEnum.Current);
                }
            }
        }

        /// <summary>
        /// Takes two sequences and combines them element-by-element into a dictionary
        /// </summary>
        public static Dictionary<TThis, TOther> ZipToDict<TThis, TOther>
            (this IEnumerable<TThis> source, IEnumerable<TOther> other)
        {            
            return source.Zip(other).ToKeyValues().ToDictionary();
        }

        /// <summary>
        /// Are all the elements in the sequence the same value?
        /// </summary>
        public static bool AreAllEqual<T>(this IEnumerable<T> source)
        {
            if (!source.Any())
                return true;

            T first = source.First();
            foreach (T item in source.Rest())
                if (!first.Equals(item))
                    return false;

            return true;
        }

        /// <summary>
        /// Pretty-print a list
        /// </summary>
        public static string Print<T>(this IEnumerable<T> source)
        {
            return "(" + source.Select(t => t.Print()).Join(",") + ")";
        }
        /// <summary>
        /// Pretty-print a list of lists
        /// </summary>
        public static string Print<T>(this IEnumerable<IEnumerable<T>> source)
        {
            return "(" + source.Select(t => t.Print()).Join(",") + ")";
        }
        /// <summary>
        /// Pretty-print a list of list of dictionaries
        /// </summary>
        public static string Print<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            return dictionaries.Select(d => d.Print()).Print();
        }

        /// <summary>
        /// Perform some function fn on each item of the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        public static void Each<T>(this IEnumerable<T> source, Action<T> fn)
        {
            foreach (var item in source)
                fn(item);
        }

        /// <summary>
        /// Convert an IEnumerable into a list - handy for working with some of .NET's built-in collection types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToList<T>(this System.Collections.IEnumerable source)
        {
            foreach (var t in source)
                yield return (T)t;
        }

        /// <summary>
        /// Perform some function fn on each item of the list, where the first argument
        /// of the function is the index of the element
        /// </summary>        
        public static void For<T>(this IEnumerable<T> source, Action<int, T> fn)
        {
            var pairs = source.Zip(0.Inc(), (t, i) => new Tuple<int, T>(i, t));
            foreach (var pair in pairs)
                fn(pair.Item1, pair.Item2);
        }

        /// <summary>
        /// Returns the first index of each of the specified elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns>A list of the first index of each item, in the order they were supplied.
        /// -1 indicates the item wasn't found</returns>
        public static IEnumerable<int> IndexOfEach<T>(this IEnumerable<T> source, params T[] items)
        {
            return IndexOfEach(source, items as IEnumerable<T>);
        }

        /// <summary>
        /// Returns the first index of each of the specified elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns>A list of the first index of each item, in the order they were supplied.
        /// -1 indicates the item wasn't found</returns>
        public static IEnumerable<int> IndexOfEach<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            // Create a local array, cuz we'll need to look this stuff up frequently,
            // and we don't want to do a potentially slow iteration over some unknown
            // collection each time
            var myItems = items.ToArray();
            var indices = new int[myItems.Length];
            // Initialize the all the indicies to -1
            0.To(myItems.Length - 1).Each(i => indices[i] = -1);

            int curLoc = 0;
            foreach (T cur in source)
            {                
                // See if the cur item is any of the items we're looking for,
                // if so, then set the index
                for (int tLoc = 0; tLoc < myItems.Length; tLoc++)
                    if (indices[tLoc] == -1 && cur.Equals(myItems[tLoc]))
                        indices[tLoc] = curLoc;

                curLoc++;

                // Optimization - if we've found all of the indices, bail out of the foreach early      
                if (indices.All(i => i != -1))
                    break;
            }

            return indices;
        }

        /// <summary>
        /// Take all the items and move them by offset in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items">The items to move</param>
        /// <param name="offset">How far to move each item</param>
        /// <returns></returns>
        public static IEnumerable<T> Shift<T>(this IEnumerable<T> source, IEnumerable<T> items, int offset)
        {
            var locations = items.ZipToDict(source.IndexOfEach(items));
            var notFounds = locations.Where(kv => kv.Value == -1);

            // Apply the offset
            // This should be an int->T mapping of the new location for each item
            var newLocations = locations.Except(notFounds).Select(kv => new KeyValuePair<T, int>(kv.Key, kv.Value + offset)).ToDictionary().Invert();

            var nonItems = source.Except(items);

            // The starting location is either 0, or the smallest new location (if we're
            // shifting some elements to below index 0)
            var start = new int[]{newLocations.Keys.Min(), 0}.Min();

            // Start at the smallest index
            // Iterate through the nonItems, inserting the items at the appropriate indices
            int cur = start;
            foreach(var nonItem in nonItems)
            {
                while(newLocations.ContainsKey(cur))
                {
                    yield return newLocations[cur];
                    cur++;
                }
                yield return nonItem;
                cur++;
            }

            // we might have some items left - these go after the end of the 
            // non-items. Return those here
            foreach(var after in newLocations.Where(kv => kv.Key >= cur).OrderBy(kv => kv.Key))
                yield return after.Value;
        }


        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(s => s);
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int times)
        {
            return Repeat(source).Take(times);
        }

        /// <summary>
        /// Turn a nested list into a 1-dimensional list
        /// </summary>
        /// <example>((a,b),(c,d)) -> (a,b,c,d)</example>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> lists)
        {
            foreach (var list in lists)
                foreach (T t in list)
                    yield return t;
        }

        /// <summary>
        /// Returns a subrange of the list from start (inclusive) to end (inclusive)
        /// </summary>
        /// <remarks>If end is less than start, the range is returned in reverse order</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="start">The index of the first element to get</param>
        /// <param name="end">The last element to get</param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> list, int start, int end)
        {            
            if(start == end)
                return list.ElementAt(start).List();

            var endpoints = new int[]{start, end};
            var a = endpoints.Min();
            var b = endpoints.Max();

            var range = list.Skip(a).Take(b-a+1);
            if(end < start)
                range = range.Reverse();

            return range;
        }

        /// <summary>
        /// Remove the last element of the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> Chop<T>(this IEnumerable<T> list)
        {
            if (!list.Any() || !list.Rest().Any()) // If it's 0 or 1 element
                yield break; // Return an empty list
            else
                foreach (var t in list.Slice(0, list.Count() - 2))// Get everything except the last element
                    yield return t;

        }
        /// <summary>
        /// Are the two sequences identical?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="otherList"></param>
        /// <returns></returns>
        public static bool IsSameAs<T>(this IEnumerable<T> list, IEnumerable<T> otherList) where T: IComparable
        {
            // Evaluate each pair using the comparison. results will contain a list of booleans, with
            // each element indicating if the corresponding elements in list and otherlist matched.
            // ie, results[i] being true means that list[i] is the same as otherList[i], according to IComparable
            var results = list.Zip(otherList, (a, b) => (a as IComparable).CompareTo(b) == 0);
            // We're ok if everything is true, and the length is the same as the original one
            var lengths = new int[] { results.Count(), list.Count(), otherList.Count() };
            return lengths.AreAllEqual() && results.AreAllEqual() && results.First() == true;
        }

        /// <summary>
        /// Makes the sequence either longer or shorter to match the length (LINQ version only shortens)
        /// If the list is too short, return paddingValue until we reach the requested length
        /// </summary>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> list, int length, T paddingValue)
        {
            int count = 0;
            foreach (var t in list)
            {                
                yield return t;
                count++;
                if (count == length)
                    break;
            }

            while (count < length)
            {
                yield return paddingValue;
                count++;
            }
        }

        /// <summary>
        /// Return items from a list until haltFn returns true - returns the item that returned true as well
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="haltFn"></param>
        /// <returns></returns>
        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> list, Func<T, bool> haltFn)
        {
            foreach (var t in list)
            {
                yield return t;
                if (haltFn(t))
                    yield break;
            }
        }

        /// <summary>
        /// Returns the first item of list, first of other, second of list, and so on
        /// Stops when either list reaches the end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IEnumerable<T> Interleave<T>(this IEnumerable<T> list, IEnumerable<T> other)
        {
            var listIter = list.GetEnumerator();
            var otherIter = other.GetEnumerator();
            while (listIter.MoveNext() && otherIter.MoveNext())
            {
                yield return listIter.Current;
                yield return otherIter.Current;
            }
        }

        /// <summary>
        /// Insert seperator between every element of list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static IEnumerable<T> Interpose<T>(this IEnumerable<T> list, T separator)
        {
            // Swap the order of the lists, then interleave, then drop the first item
            // That way, we don't end up with a trailing seperator
            // (because Interleave will return (s,1,s,2,s,3) and we want (1,s,2,s,3)
            return Interleave(new T[] { separator }.Repeat(), list).Skip(1);
        }

        /// <summary>
        /// Calculates the interval between each pair of successive elements in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="intervalFunc">A function that takes two subsequent elements in the list and
        /// calculates the difference between them</param>
        /// <returns>A list with one less element than the original list (since an list of length
        /// n has n-1 intervals between its elements)</returns>
        public static IEnumerable<TResult> Intervals<T, TResult>(
            this IEnumerable<T> list,
            Func<T, T, TResult> intervalFunc)
        {            
            var prev = list.First();
            // Go from item #2 to the end of the list, calling the fn on each pair of items
            foreach (var cur in list.Rest())
            {
                yield return intervalFunc(prev, cur);
                prev = cur;
            }
        }

        /// <summary>
        /// Turns a list into a list of lists, each of size
        /// The last list may be shorter, if there are not enough elements to make a full list
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> list, int size)
        {
            // Get the first partition,            
            var res = list.Take(size);

            // As long as the res still has elements in it,
            // chop off the first {size} elements, and take {size} more to be res
            while (res.Count() > 0)
            {
                yield return res;
                list = list.Skip(size);
                if (list.Any())
                    res = list.Take(size);
                else
                    res = new T[] { };
            }
        }

        /// <summary>
        /// Turns the list into a list of lists, the first being of length sizes.First(), 
        /// the second being of length sizes.Second(), and so on
        /// </summary>
        /// <remarks>The last list may be shorter than specified, if there are not enough
        /// elements. If there are not enough sizes specified, they will be repeated</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> list, IEnumerable<int> sizes)
        {
            var amountsToTake = sizes.Repeat();
            var remaining = list;            
            var amtIterator = amountsToTake.GetEnumerator();
            amtIterator.MoveNext(); // Move to the first element
            
            while (remaining.Any())
            {
                var res = remaining.Take(amtIterator.Current);
                remaining = remaining.Skip(amtIterator.Current);
                yield return res;
                amtIterator.MoveNext();
            }
        }

        /// <summary>
        /// Similar to Aggregate, but returns all the intermediate results
        /// </summary>
        /// <remarks>Stolen from Scala 2.8's ScanLeft/ScanRight</remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="list"></param>
        /// <param name="initialValue"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Scan<T, TResult>(
            this IEnumerable<T> list, TResult initialValue,
            Func<TResult, T, TResult> fn)
        {
            var cur = initialValue;
            foreach (var item in list)
            {
                cur = fn(cur, item);
                yield return cur;
            }
        }

        /// <summary>
        /// Weights each item in list according to sizeFn, then takes items until the total
        /// weight reaches amtToScoop. The result is a mapping of each item to the amount
        /// we need to take from the item to achieve the target weight
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="list"></param>
        /// <param name="sizeFn"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T, int>> Scoop<T>(this IEnumerable<T> list, int amtToScoop, Func<T, int> sizeFn)
        {   
            var sum = 0;
            foreach (var item in list)
            {
                var itemSize = sizeFn(item);
                var newSum = sum + itemSize;
                if (newSum < amtToScoop)
                    yield return Tuple.Create(item, itemSize);
                else
                {
                    yield return Tuple.Create(item, amtToScoop - sum);
                    yield break;
                }
                sum = newSum;
            }
        }

        /// <summary>
        /// Weights each item in list according to sizeFn, then takes items until the total
        /// weight reaches amtToScoop. The result is a mapping of each item to the amount
        /// we need to take from the item to achieve the target weight
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="list"></param>
        /// <param name="sizeFn"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T, decimal>> Scoop<T>(this IEnumerable<T> list, decimal amtToScoop, Func<T, decimal> sizeFn)
        {
            var sum = 0m;
            foreach (var item in list)
            {
                var itemSize = sizeFn(item);
                var newSum = sum + itemSize;
                if (newSum < amtToScoop)
                    yield return Tuple.Create(item, itemSize);
                else
                {
                    yield return Tuple.Create(item, amtToScoop - sum);
                    yield break;
                }
                sum = newSum;
            }
        }

        /// <summary>
        /// Gets every nth item in the list
        /// ie, [1,2,3,4].TakeNth(2) -> [2,4]
        /// </summary>
        /// <remarks>Stolen from Clojure</remarks>
        public static IEnumerable<T> TakeNth<T>(this IEnumerable<T> list, int n)
        {
            int count = list.Count();
            for (int i = n-1; i < count; i += n)
                yield return list.ElementAt(i);
        }

        /// <summary>
        /// Divides list into logical groups with partitionFn, then return complete groups
        /// from the list, supplying at least minNumtoTake (unless the list is shorter than that).
        /// Basically, this will take up until the first point after minNumToTake where partitionFn changes values
        /// </summary>
        /// <remarks>See tests for examples</remarks>
        public static IEnumerable<T> TakeByGroup<T, TGroup>(this IEnumerable<T> list, Func<T, TGroup> partitionFn, int minNumToTake)
        {
            if(!list.Any() || minNumToTake == 0)
                yield break;

            TGroup lastGroup = default(TGroup);
            int taken = 0;
            foreach (var item in list)
            {                
                if(++taken == minNumToTake)
                    lastGroup = partitionFn(item);

                if (taken <= minNumToTake || partitionFn(item).Eq(lastGroup))
                    yield return item;
                else
                    yield break;
            }
        }

        /// <summary>
        /// Return the element from the list that has the largest value for the selector Fn
        /// If more than one element has the max value, only one of them will be returned (which one
        /// should not be relied on)
        /// </summary>
        public static T HavingMax<T, TResult>(
            this IEnumerable<T> list,
            Func<T, TResult> selector)
        {
            var index = list.Index(selector);
            var biggestIndex = index.Keys.Max();
            return index[biggestIndex].First();
        }

        /// <summary>
        /// Returns true if the list contains at least count items
        /// </summary>
        /// <remarks>Only realizes enough of the sequence to check its condition, then stops, so this is potentially
        /// much more efficient than calling Count on a sequence</remarks>
        public static bool AtLeast<T>(this IEnumerable<T> list, int count)
        {
            var iter = list.GetEnumerator();
            var spot = 0;
            while (iter.MoveNext() && ++spot <= count) ;
            return (spot >= count);
        }

        /// <summary>
        /// Returns true if the list contains at least count items that pass the predicate
        /// </summary>
        /// <remarks>Only realizes enough of the sequence to check its condition, then stops, so this is potentially
        /// much more efficient than calling Where(predicate).Count() on a sequence</remarks>
        public static bool AtLeast<T>(this IEnumerable<T> list, int count, Func<T, bool> predicate)
        {
            var found = 0;
            foreach (var item in list)
            {
                if (predicate(item))
                    ++found;
                if (found >= count)
                    return true;
            }

            return false;
        }

        #region Infinite sequences
        /// <summary>
        /// Repeats the sequence forever. WARNING: Generates an infinite sequence - 
        /// use a Take() or something on it, or your program will be stuck here forever
        /// </summary>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source)
        {
            while (true)
                foreach (var t in source)
                    yield return t;
        }

        /// <summary>
        /// Returns an infinite sequence of x, f(x), f(f(x)), etc
        /// </summary>
        /// <remarks>Stolen shamelessy from Clojure</remarks>
        public static IEnumerable<T> Iterate<T>(this T start, Func<T, T> f)
        {
            var cur = start;
            while (true)
            {
                yield return cur;
                cur = f(cur);
            }
        }

        #endregion

        #region Functions that work on individual objects, but are list-related
        /// <summary>
        /// Works like the SQL 'IN' keyword and returns true if your object is contained inside any of the passed in values
        /// </summary>
        /// <values>You can pass in multiple values such as strings separated by commas</values>
        public static bool In<T>(this T testValue, params T[] values)
        {
            return values.Contains(testValue);
        }

        /// <summary>
        /// Convert an item into a 1-element list
        /// </summary>
        public static IEnumerable<T> List<T>(this T source)
        {
            yield return source;
        }

        /// <summary>
        /// Prepends an item onto the front of a list AKA "cons" from LISP
        /// </summary>
        public static IEnumerable<T> And<T>(this T source, IEnumerable<T> items)
        {
            yield return source;
            foreach (var i in items)
                yield return i;
        }

        /// <summary>
        /// Combines two single elements into a list
        /// </summary>
        public static IEnumerable<T> And<T>(this T source, T other)
        {
            yield return source;
            yield return other;
        }

        /// <summary>
        /// Adds another element to the end of a list
        /// </summary>
        public static IEnumerable<T> And<T>(this IEnumerable<T> source, T other)
        {
            foreach (var t in source)
                yield return t;
            yield return other;
        }
        
        #endregion
    }
}
