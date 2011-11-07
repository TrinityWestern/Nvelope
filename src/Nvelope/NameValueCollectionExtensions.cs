using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Nvelope
{    
    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var res = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                if (key == null)
                    res.Add("", "");
                else
                    res.Add(key, collection[key]);
            }
            return res;
        }

        public static IEnumerable<string> Enumerate(this NameObjectCollectionBase.KeysCollection keys)
        {
            foreach (string key in keys)
                yield return key;
        }
        public static string Print(this NameValueCollection collection)
        {
            return collection.ToDictionary().Print();
        }

        public static bool ContainsKey(this NameValueCollection collection, string key)
        {
            return collection.AllKeys.Contains(key);
        }

        public static NameValueCollection ToNameValueCollection(this string source, string keyValueSeparator, string itemSeparator)
        {
            NameValueCollection res = new NameValueCollection();

            string[] items = source.Split(itemSeparator);

            foreach (string item in items)
            {
                if (item.Length != 0)
                {
                    string[] itemdata = item.SplitPair(keyValueSeparator);
                    res.Add(itemdata.First(), itemdata.Last());
                }
            }

            return res;
        }

        /// <summary>
        /// Returns a new dictionary that excludes specific keys
        /// </summary>
        public static Dictionary<string, string> Except(this NameValueCollection collection, IEnumerable<string> keysToExclude)
        {
          return  collection.ToDictionary().Except(keysToExclude);    
        }

        /// <summary>
        /// Returns a new dictionary that excludes specific keys
        /// </summary>
        public static Dictionary<string, string> Except(this NameValueCollection collection, params string[] keysToExclude)
        {
            return Except(collection, keysToExclude as IEnumerable<string>);
        }


    }
}
