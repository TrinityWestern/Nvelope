using System;
using System.Collections.Generic;
using System.Linq;
using Nvelope.Reflection;

namespace Nvelope
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Combines two dictionaries - if both dictionaries define the same key, second one wins
        /// </summary>
        public static Dictionary<TKey, TValue> Union<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other)
        {
            if (other.Count == 0)
                return dict;

            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>(dict);
            foreach (TKey key in other.Keys)
                if (!res.ContainsKey(key))
                    res.Add(key, other[key]);
                else
                    res[key] = other[key];

            return res;
        }
        
        /// <summary>
        /// Swaps the keys and values
        /// </summary>
        /// <remarks>All the values must be unique, or an exception will be thrown. To check, call IsOneToOne</remarks>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            Dictionary<TValue, TKey> res = new Dictionary<TValue, TKey>();
            foreach (TKey key in dict.Keys)
                res.Add(dict[key], key);
            return res;
        }

        /// <summary>
        /// Is every value unique?
        /// </summary>
        public static bool IsOneToOne<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict.Values.Distinct().Count() == dict.Keys.Count();
        }

        /// <summary>
        /// Converts to a user-friendly representation
        /// </summary>
        public static string Print<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            if (dict == null)
                return "";

            IEnumerable<KeyValuePair<TKey, TValue>> orderedDict = dict;
            // If we can order the keys, do so
            if (dict.Any())
            {
                var asIComp = dict.Keys.First() as IComparable;
                if (asIComp != null) // If we can't convert to IComparable, so we can't sort
                    orderedDict = dict.OrderBy(kv => kv.Key);
            }

            return orderedDict.Select(kv => "[" + kv.Key.Print() + "," + kv.Value.Print() + "]").Print();
        }

        /// <summary>
        /// Converts to a user-friendly representation
        /// </summary>
        public static string Print<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict)
        {
            return dict.OrderBy(kv => kv.Key).Select(kv => "[" + kv.Key.Print() + "," + kv.Value.Print() + "]").Print();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValues)
        {
            var res = new Dictionary<TKey, TValue>();
            foreach (var kv in keyValues)
                res.Add(kv.Key, kv.Value);
            return res;
        }

        /// <summary>
        /// Get a sequence of all the values for a given collection of keys
        /// </summary>
        public static IEnumerable<TValue> ValuesForKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            foreach (TKey key in keys)
                if(dict.ContainsKey(key))
                    yield return dict[key];
        }

        /// <summary>
        /// Get a sequence of all the values for a given collection of keys
        /// </summary>
        public static IEnumerable<TValue> ValuesForKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] keys)
        {
            return ValuesForKeys(dict, keys as IEnumerable<TKey>);
        }

        /// <summary>
        /// Are the two dictionaries the same? Compares only on the keys of the first dictionary - if the second has
        /// additional keys, they are ignored
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsSameAs<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other)
        {
            return IsSameAs(dict, other, dict.Keys);
        }

        /// <summary>
        /// Compare two dictionaries and return the differences
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="other"></param>
        /// <param name="fields"></param>
        /// <param name="nullVal">If one dict doesn't define a field, return this as the value for that dict for that field</param>
        /// <returns></returns>
        public static Dictionary<TKey, Tuple<TValue, TValue>> Diff<TKey, TValue>(
            this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other, IEnumerable<TKey> fields = null, TValue nullVal = default(TValue))
        {
            if(fields == null || !fields.Any())
                fields = dict.Keys.Union(other.Keys);

            // Get the values that exist only in dict
            var onlyA = dict.Keys.Except(other.Keys).Only(fields).ToSet();

            // Get the values that exist only in other
            var onlyB = other.Keys.Except(dict.Keys).Only(fields).ToSet();

            // Get the values that are different in dict and other
            // These are the fields that exist on dict and other and are contained in fields
            var comparisonKeys = dict.Keys.Only(fields).Except(onlyA).Except(onlyB).ToSet();
            var neq = comparisonKeys.Where(k => dict[k].Neq(other[k]));

            var res = neq.ToDictionary(s => s, s => Tuple.Create(dict[s], other[s]))
                .Union(onlyA.ToDictionary(s => s, s => Tuple.Create(dict[s], nullVal)))
                .Union(onlyB.ToDictionary(s => s, s => Tuple.Create(nullVal, other[s])));

            return res;
        }

        /// <summary>
        /// Are the two dictionaries the same? Compares on the supplied keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="other"></param>
        /// <param name="comparisonKeys"></param>
        /// <returns></returns>
        public static bool IsSameAs<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other, IEnumerable<TKey> comparisonKeys)
        {
            var myKeys = dict.Keys.Where(k => comparisonKeys.Contains(k));
            var otherKeys = other.Keys.Where(k => comparisonKeys.Contains(k));
            if (myKeys.Count() != comparisonKeys.Count())
                return false;
            if (otherKeys.Count() != comparisonKeys.Count())
                return false;

            foreach (var key in comparisonKeys)
                if (!otherKeys.Contains(key))
                    return false;
                else if (!myKeys.Contains(key))
                    return false;
                else if (dict[key] == null && other[key] != null)
                    return false;
                else if (dict[key] == null && other[key] == null)
                    return true;
                else if (dict[key] != null && other[key] == null)
                    return false;
                else if (!dict[key].Eq(other[key]))
                    return false;

            return true;
        }

        /// <summary>
        /// Are the two dictionaries the same? Compares on the supplied keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="other"></param>
        /// <param name="comparisonKeys"></param>
        /// <returns></returns>
        public static bool IsSameAs<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other, params string[] comparisonKeys)
        {
            return IsSameAs(dict, other, comparisonKeys as IEnumerable<TKey>);
        }


        public static IDictionary<TKey, TValue> RemoveWhereKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TKey, bool> keyFilter)
        {
            var victims = dict.Keys.Where(k => keyFilter(k)).ToList();
            foreach (var vic in victims)
                dict.Remove(vic);
            return dict;
        }

        public static IDictionary<TKey, TValue> RemoveWhereValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TValue, bool> valueFilter)
        {
            var victims = dict.Where(kv => valueFilter(kv.Value)).Select(kv => kv.Key).ToList();
            foreach (var vic in victims)
                dict.Remove(vic);
            return dict;
        }

        /// <summary>
        /// Converts the keys of the dictionary using the supplied converter function
        /// </summary>
        public static Dictionary<TNewKey, TValue> SelectKeys<TKey, TValue, TNewKey>(this IDictionary<TKey, TValue> dict, Func<TKey, TNewKey> keySelector)
        {
            return Select(dict, keySelector, v => v);
        }
        /// <summary>
        /// Converts the keys and values of the dictionary using the supplied converter functions
        /// </summary>
        public static Dictionary<TNewKey, TNewValue> Select<TKey, TValue, TNewKey, TNewValue>(this IDictionary<TKey, TValue> dict, Func<TKey, TNewKey> keySelector, Func<TValue, TNewValue> valueSelector)
        {
            Dictionary<TNewKey, TNewValue> res = new Dictionary<TNewKey, TNewValue>();
            foreach (var kv in dict)
                res.Add(keySelector(kv.Key), valueSelector(kv.Value));
            return res;
        }

        /// <summary>
        /// Converts the values of the dictionary using the supplied converter function
        /// </summary>
        public static Dictionary<TKey, TNewValue> SelectVals<TKey, TValue, TNewValue>(this IDictionary<TKey, TValue> dict, Func<TValue, TNewValue> valueSelector)
        {
            return dict.Select(k => k, v => valueSelector(v));
        }

        /// <summary>
        /// Creates or updates all the key-value pairs in the dictionary to be 
        /// those of the second dictionary
        /// </summary>
        public static void Update<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other)
        {
            foreach (var key in other.Keys)
                if (!dict.ContainsKey(key))
                    dict.Add(key, other[key]);
                else
                    dict[key] = other[key];
        }

        /// <summary>
        /// If any of the values of the dictionary are Func{object}, execute the Func and
        /// return the resultant value instead of the Func{object}
        /// </summary>
        public static Dictionary<TKey, object> Realize<TKey>(this Dictionary<TKey, object> dict)
        {
            return dict.SelectVals(o => o.Realize());
        }


        /// <summary>
        /// Returns a new dictionary that excludes specific keys
        /// </summary>
        public static Dictionary<TKey, TValue> Except<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<TKey> keysToExclude)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            foreach (TKey key in dict.Keys)
                if (!keysToExclude.Contains(key))
                    res.Add(key, dict[key]);
                
            return res;
        }

        /// <summary>
        /// Returns a new dictionary that excludes specific keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="keysToExclude"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Except<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] keysToExclude)
        {
            return Except(dict, keysToExclude as IEnumerable<TKey>);
        }

        /// <summary>
        /// Returns a new dictionary containing only the specified keys
        /// </summary>
        public static Dictionary<TKey, TValue> Only<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<TKey> keysToInclude)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            foreach (TKey key in keysToInclude)
                if (dict.ContainsKey(key))
                    res.Add(key, dict[key]);

            return res;
        }

        /// <summary>
        /// Returns a new dictionary containing only the specified keys
        /// </summary>
        public static Dictionary<TKey, TValue> Only<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] keysToInclude)
        {
            return Only(dict, keysToInclude as IEnumerable<TKey>);
        }

        /// <summary>
        /// Add a bunch of key-value pairs to a dictionary
        /// </summary>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            items.Each(i => dict.Add(i));
        }

        /// <summary>
        /// Return a new dictionary with the extra key added. If the key already exists, it's value is changed in the new dictionary
        /// </summary>
        /// <remarks>This makes it easier to do immutable programming with dictionaries. 
        /// Shamelessly stolen from clojure</remarks>
        public static Dictionary<TKey, TValue> Assoc<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey newKey, TValue newValue)
        {
            var res = new Dictionary<TKey, TValue>(dict);
            res.SetVal(newKey, newValue);
            return res;
        }

        /// <summary>
        /// Makes sure that the dictionary contains key, and then sets it to value
        /// </summary>
        public static void Ensure<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] = value;
        }

        /// <summary>
        /// Returns the value if the corresponding key is in the dictionary, else returns defaultValue
        /// </summary>
        public static TValue Val<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return defaultValue;
        }

        /// <summary>
        /// Returns the value if the corresponding key is in the dictionary, else returns the default for TValue
        /// </summary>
        public static TValue Val<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return default(TValue);
        }

        /// <summary>
        /// Set the value if it exists, otherwise add it
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Dictionary<TKey,TValue> SetVal<TKey,TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, val);
            else
                dict[key] = val;

            return dict;
        }

        /// <summary>
        /// Constructs a single object of type T from the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T Read<T>(this Dictionary<string, object> dict) where T : class, new()
        {
            ObjectReader<T> reader = new ObjectReader<T>();
            return reader.Read(dict);
        }

        /// <summary>
        /// Returns a deep copy of the dictionary
        /// </summary>
        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            foreach (var key in dict.Keys)
                res.Add(key, dict[key]);
            return res;
        }

        /// <summary>
        /// Creates a new dictionary, containing only the keys that pass the predicate
        /// </summary>
        public static Dictionary<TKey, TValue> WhereKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TKey, bool> predicate)
        {
            var res = new Dictionary<TKey, TValue>();
            foreach (var key in dict.Keys.Where(predicate))
                res.Add(key, dict[key]);

            return res;
        }

        /// <summary>
        /// Creates a new dictionary, containing only the items whose values passed the predicate
        /// </summary>
        public static Dictionary<TKey, TValue> WhereValues<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TValue, bool> predicate)
        {
            var res = new Dictionary<TKey, TValue>();
            // Apply the predicate to every key-value pair in the dict, and add any that pass to the result
            dict.Where(kv => predicate(kv.Value)).Each(kv => res.Add(kv.Key, kv.Value));

            return res;
        }

        /// <summary>
        /// Execute an action for each key-value pair in the dictionary
        /// </summary>
        public static void Each<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey,TValue> action)
        {
            foreach (var kv in dict)
                action(kv.Key, kv.Value);
        }

        /// <summary>
        /// Represet the keys of a dictionary as a set.
        /// </summary>
        /// <remarks>
        /// For most operations on a dictionary's keys, treating them like a
        /// set makes the most sense. This method just makes that simpler.
        /// </remarks>
        public static HashSet<TKey> ToSet<TKey, TValue>(
            this Dictionary<TKey, TValue>.KeyCollection keys)
        {
            return new HashSet<TKey>(keys);
        }
    }
}
