using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Collections
{
    /// <summary>
    /// A dictionary class that provides hooks on adding, removing, and altering items.
    /// Also, it does not throw KeyNotFoundExceptions - always returns a value, and you 
    /// can specify the default value to return if the key is not in the dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Obsolete("If you're using this class for it's implicit adding behavior, use a standard dictionary with the SetVal and Val extension methods instead")]
    public class SDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// SDictionary with the NullValue that equals default(TValue)
        /// </summary>
        public SDictionary()
        {
            ItemAdded += dummyAltered;
            ItemRemoved += dummyAltered;
            ItemSet += dummyAltered;
            ItemAdding += dummyAltering;
            ItemRemoving += dummyAltering;
            ItemSetting += dummyAltering;
        }

        #region Events
        /// <summary>
        /// Fired before an item is added. If it returns false, the add will be cancelled
        /// </summary>
        public event DictItemAltering ItemAdding;
        /// <summary>
        /// Fired after an item is added.
        /// </summary>
        public event DictItemAltered ItemAdded;
        /// <summary>
        /// Fired before an item is removed. If it returns false, the remove will be cancelled.
        /// </summary>
        public event DictItemAltering ItemRemoving;
        /// <summary>
        /// Fired after an item is removed
        /// </summary>
        public event DictItemAltered ItemRemoved;
        /// <summary>
        /// Fired before an item is altered. If it returns false, the alter will be cancelled.
        /// </summary>
        public event DictItemAltering ItemSetting;
        /// <summary>
        /// Fired after an item is altered.
        /// </summary>
        public event DictItemAltered ItemSet;
        #endregion

        #region Default Value
        /// <summary>
        /// Construct a new dictionary, using nullValue as the default value instead of default(T)
        /// </summary>
        /// <param name="nullValue"></param>
        /// <returns></returns>
        public static SDictionary<TKey, TValue> WithNull(TValue nullValue)
        {
            var res = new SDictionary<TKey, TValue>();
            res.NullValue = nullValue;
            return res;
        }

        /// <summary>
        /// The value to use in place of null, when null would normally be returned
        /// </summary>
        public TValue NullValue = default(TValue);
        #endregion

        #region IDictionary<TKey,TSource> Members

        public void Add(TKey key, TValue value)
        {
            if (ItemAdding(new DictItemChange(key, value, this)))
            {
                _dict.Add(key, value);
                ItemAdded(new DictItemChange(key, value, this));
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _dict.Keys; }
        }
        public bool Remove(TKey key)
        {
            if (ItemRemoving(new DictItemChange(key, this[key], this)))
            {
                TValue val = _dict[key];
                bool res = _dict.Remove(key);
                ItemRemoved(new DictItemChange(key, val, this));
                return res;
            }
            return false;
        }
        /// <summary>
        /// Warning, this may give you confusing results because it doesn't
        /// use NullValue for items that aren't set.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _dict.Values; }
        }

        public TValue this[TKey key]
        {
            get {
                if (_dict.ContainsKey(key))
                    return _dict[key];
                else
                    return NullValue;
            }
            set {
                if (ItemSetting(new DictItemChange(key, value, this))) {
                    _dict[key] = value;
                    ItemSet(new DictItemChange(key, value, this));
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TSource>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            var keys = this.Keys.ToList();

            foreach (var key in keys)
                Remove(key);
        }
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return _dict.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (ItemRemoving(new DictItemChange(item.Key, item.Value, this)))
            {
                ItemRemoved(new DictItemChange(item.Key, item.Value, this));
                return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        #region Implementation
        private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
        private bool dummyAltering(DictItemChange change) { return true; }
        private void dummyAltered(DictItemChange change) { }
        #endregion

        #region
        /// <summary>
        /// Delegate type for post-action hooks
        /// </summary>
        /// <param name="change"></param>    
        public delegate void DictItemAltered(DictItemChange change);

        /// <summary>
        /// Delegate type for pre-action hooks
        /// </summary>
        /// <param name="change"></param>
        /// <returns>True if the operation should be allowed to procede, false to cancel</returns>
        public delegate bool DictItemAltering(DictItemChange change);

        /// <summary>
        /// Encapsulates a change to a list
        /// </summary>    
        public class DictItemChange
        {  
            public DictItemChange(TKey key, TValue value, SDictionary<TKey, TValue> dictionary)
            {
                Key = key;
                Value = value;
                Dictionary = dictionary;
            }
            /// <summary>
            /// The key being altered
            /// </summary>
            public TKey Key { get; set; }

            /// <summary>
            /// The value being altered
            /// </summary>
            public TValue Value { get; set; }

            /// <summary>
            /// The list hosting the change
            /// </summary>
            public SDictionary<TKey, TValue> Dictionary { get; set; }
        }
        #endregion

    }
}
