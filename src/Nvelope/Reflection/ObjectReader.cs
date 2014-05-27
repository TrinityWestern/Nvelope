using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nvelope.Exceptions;


namespace Nvelope.Reflection
{
    public class ObjectReader
    {
        /// <summary>
        /// If not null, the reader will set just those properties that have this attribute 
        /// type on them
        /// </summary>
        public Type AttributeType;
        /// <summary>
        /// If AttributeType is not null, should we look up the inheritance hierarchy to find
        /// properties with that attribute?
        /// </summary>
        public bool IncludeInheritedAttributes = true;
        /// <summary>
        /// Should we set fields on the destination object
        /// </summary>
        public bool SetFields = true;
        /// <summary>
        /// Should we set properties on the destination object
        /// </summary>
        public bool SetProperties = true;
        /// <summary>
        /// Are the field names case sensitive
        /// </summary>
        public bool IsCaseSensitive = false;
        /// <summary>
        /// Some action to run on every object after we finish writing to it
        /// </summary>
        public Action<object> PostReadAction;
        /// <summary>
        /// Some action to run on every object before we write to it
        /// </summary>
        public Action<object> PreReadAction;
        /// <summary>
        /// This function is executed if the reader doesn't contain a field that it expects.
        /// It should return a value for the field
        /// </summary>
        public Func<Dictionary<string, object>, string, Type, object> OnMissingValue = _defaultOnMissingValue;

        private static object _defaultOnMissingValue(Dictionary<string, object> rowContents, string expectedField, Type expectedType)
        {
            return null;
        }

        /// <summary>
        /// Set the properties/fields on instance from data
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public object Read(object instance, Dictionary<string, object> data)
        {
            if (PreReadAction != null)
                PreReadAction(instance);
            _set(instance, data);
            if (PostReadAction != null)
                PostReadAction(instance);
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<object> ReadAll(Type type, IEnumerable<Dictionary<string, object>> data)
        {
            List<object> res = new List<object>();
            foreach (var row in data)
                res.Add(Read(Activator.CreateInstance(type), row));

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="reader"></param>
        /// <param name="readerFields"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        protected object _set(object instance, Dictionary<string, object> data)
        {
            // TODO: Rename this method to make it CLS compliant
            var include = MemberTypes.Field | MemberTypes.Property;
            if (!this.SetFields)
                include &= ~MemberTypes.Field;
            if (!this.SetProperties)
                include &= ~MemberTypes.Property;

            var members = instance._GetMembers().RemoveReadOnly();
            if (this.AttributeType != null) {
                var mi = typeof(ReflectionExtensions).GetMethod("FilterAttributeType");
                var filterRef = mi.MakeGenericMethod(this.AttributeType);
                var args = new object[] { this.IncludeInheritedAttributes };
                filterRef.Invoke(members, args);
            }
            
            var decalaration = members.FieldDeclarations();
            foreach (var kv in decalaration)
            {
                var fieldName = kv.Key;
                var fieldType = kv.Value;
                string dataField = fieldName;
                if (!IsCaseSensitive && data.Keys.Where(r => r.ToUpperInvariant() == fieldName.ToUpperInvariant()).Count() == 1)
                    dataField = data.Keys.First(r => r.ToUpperInvariant() == fieldName.ToUpperInvariant());

                object val;
                try
                {
                    if (data.ContainsKey(dataField))
                        val = data[dataField].ConvertTo(fieldType);
                    else
                        val = OnMissingValue(data, fieldName, fieldType);
                }
                catch (ConversionException ex)
                {
                    throw new ConversionException("Error setting object field \"" + fieldName + "\" from reader field \"" + dataField +
                        "\" on type \"" + instance.GetType().Name + "\": " + ex.Message, ex);
                }

                instance._Set(fieldName, val);
            }

            return instance;
        }
    }


    /// <summary>
    /// A class that reflectively loads a .NET type from a DataReader
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectReader<T> : ObjectReader where T : class, new()
    {
        public ObjectReader()
        { }

        public ObjectReader(ObjectReader reader)
        {
            this.AttributeType = reader.AttributeType;
            this.IncludeInheritedAttributes = reader.IncludeInheritedAttributes;
            this.IsCaseSensitive = reader.IsCaseSensitive;
            this.OnMissingValue = reader.OnMissingValue;
            this.PostReadAction = reader.PostReadAction;
            this.PreReadAction = reader.PreReadAction;
            this.SetFields = reader.SetFields;
            this.SetProperties = reader.SetProperties;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T Read(T instance, Dictionary<string, object> data)
        {
            return (T)base.Read(instance, data);
        }

        public T Read(Dictionary<string, object> data)
        {
            return (T)base.Read(Activator.CreateInstance(typeof(T)), data);
        }

        
        public List<T> ReadAll(IEnumerable<Dictionary<string, object>> data)
        {
            return base.ReadAll(typeof(T), data).Cast<T>().ToList();
        }

    }

}
