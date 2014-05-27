using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Nvelope.Exceptions;
using Nvelope.Reflection;

namespace Nvelope.Data
{
    /// <summary>
    /// Combines an object and a dictionary into a single datasource.
    /// If the dictionary contains a key, that's what will be returned, 
    /// otherwise the appropriate field/property on the object will be returned
    /// </summary>
    /// <example>You can use this class when writting web pages that don't use ASP controls.
    /// You create a new DataProxy from some source database object and the Request.Params
    /// collection. Then, in your ASP page, you bind your control values to the proxy's
    /// elements - that way, on a regular request, you get the DB values, but on a "postback",
    /// you'll get whatever values the user submitted with the form.</example>
    public class DataProxy : DataProxy<string>
    {
        public DataProxy(Dictionary<string, string> overrides, object source = null, bool returnDefaultIfMissing = false, bool returnNullForExceptions = false)
            : base(overrides, source, returnDefaultIfMissing, returnNullForExceptions)
        { }

        public DataProxy(NameValueCollection overrides, object source = null, bool returnDefaultIfMissing = false, bool returnNullForExceptions = false)
            : base(overrides.ToDictionary(), source, returnDefaultIfMissing, returnNullForExceptions)
        { }
    }

    /// <summary>
    /// Combines an object and a dictionary into a single datasource.
    /// If the dictionary contains a key, that's what will be returned, 
    /// otherwise the appropriate field/property on the object will be returned
    /// </summary>
    /// <typeparam name="TValue">The type of the data returned</typeparam>
    public class DataProxy<TValue>
    {
        public DataProxy(Dictionary<string, TValue> overrides, object source = null, bool returnDefaultIfMissing = false, bool returnNullForExceptions = false)
        {
            Source = source == null ? new { } : source;
            Overrides = overrides;
            ReturnDefaultIfMissing = returnDefaultIfMissing;
            ReturnNullForExceptions = returnNullForExceptions;
        }

        public bool ReturnDefaultIfMissing { get; set; }

        public bool ReturnNullForExceptions { get; set; }
 
        protected object Source { get; set; }

        protected IDictionary<string, TValue> Overrides { get; private set; }

        public TValue this[string name]
        {
            get
            {
                var upperName = name.ToUpperInvariant();
                var matchingKeys = Overrides.Keys.Where(k => k.ToUpperInvariant() == upperName);

                // If our overrides contain a matching key, return its value
                if (matchingKeys.Any())
                    return Overrides[matchingKeys.First()];

                // We have to treat dictionaries different than other objects to get the keys
                var isDictionary = Source.GetType() == typeof(Dictionary<string, object>);

                var sourceKeys = new List<string>();
                if (isDictionary)
                    sourceKeys = Source._AsDictionary().Select(d => d.Key.ToUpperInvariant()).ToList();
                else
                    sourceKeys = Source._GetMembers().Names().Select(s => s.ToUpperInvariant()).ToList();

                if (sourceKeys.Contains(upperName))
                {
                    if (ReturnNullForExceptions) {
                        try {
                            if (isDictionary)
                                // Here we don't want the upper-case name as it won't match the keys passed in necessarily
                                return Source._AsDictionary()[name].ConvertTo<TValue>();
                            else
                                return Source.GetFieldValue(upperName, false).ConvertTo<TValue>();
                        } catch (FieldNotFoundException) {
                            return default(TValue);
                        } catch (ArgumentException) {
                            return default(TValue);
                        } catch (ConversionException) {
                            return default(TValue);
                        }
                    } else {
                        if (isDictionary)
                            // Here we don't want the upper-case name as it won't match the keys passed in necessarily
                            return Source._AsDictionary()[name].ConvertTo<TValue>();
                        else
                            return Source.GetFieldValue(upperName, false).ConvertTo<TValue>();
                    }
                } else if (ReturnDefaultIfMissing)
                    return default(TValue);
                else
                    throw new KeyNotFoundException("Neither the override collection nor the source object contained a field called '" + name + "'");
            }
        }


    }
}
