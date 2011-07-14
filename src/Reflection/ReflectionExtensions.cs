using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nvelope.Exceptions;

namespace Nvelope.Reflection
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Returns the base type if type is nullable, otherwise returns type
        /// </summary>
        /// <example>GetNonGenericType(int?) -> int</example>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonGenericType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Name == "Nullable`1")
                // try to convert to the underlying base type instead
                type = type.GetGenericArguments()[0];

            return type;
        }


        /// <summary>
        /// Gets the property or field on the object
        /// </summary>
        /// <remarks>throws FieldNotFoundException if the field can't be found</remarks>
        /// <param name="source"></param>
        /// <param name="careAboutCase"></param>
        /// <param name="field"></param>
        /// <exception cref="FieldNotFoundException">The field isn't set on the object</exception>
        /// <exception cref="ArgumentException">The field isn't supported (an indexed field)</exception>
        /// <exception cref="Exception">I'm not really sure what this means</exception>
        /// <returns></returns>
        public static object GetFieldValue(this object source,
            string fieldName, bool careAboutCase = true)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            if (!careAboutCase)
                bindingFlags |= BindingFlags.IgnoreCase;

            Type type = source.GetType();
            var members = type.GetMember(fieldName, bindingFlags);
            if (members.Count() == 0) {
                throw new FieldNotFoundException("Missing field '" + fieldName + "'");
            }
            if (members.Count() > 1) {
                // TODO: test this
                throw new Exception("Too many members found. Don't know what this means");
            }
            var member = members[0];
            var property = member as PropertyInfo;
            if (property != null) {
                if (property.GetIndexParameters().Count() != 0)
                    throw new ArgumentException("Can't handle parameters with indexies");
                try {
                    return property.GetValue(source, null);
                } catch (TargetInvocationException) {
                    // TODO: I don't think catching this exception
                    // is something I should be doing, but I'm just doing
                    // it because of invalid objects in the
                    // SetDBFieldsFrom test in Core
                    return null;
                }
            }
            var field = member as FieldInfo;
            if (field != null)
            {
                try {
                    return field.GetValue(source);
                } catch (TargetInvocationException) {
                    return null;
                }
            }
            throw new Exception("What's going on?");
        }

        /// <summary>
        /// Gets the property or field on the object and converts to the specified type
        /// </summary>
        public static T GetFieldValue<T>(this object source, string fieldName)
        {
            return (T)GetFieldValue(source, fieldName).ConvertTo(typeof(T));
        }

        /// <summary>
        /// Get and object's members.
        /// 
        /// By default it only gets the public instance fields and properties.
        /// </summary>
        /// <remarks>
        /// For types, the only types that are supported are constructors,
        /// events, fields, methods, and properties. Nested types,
        /// TypeInfos, and custom member types are not supported.
        /// </remarks>
        /// <param name="source">Get the members from this object</param>
        /// <param name="types">Flags for the types of fields returned</param>
        /// <param name="binding">Miscellaneous options</param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> _GetMembers(this object obj,
            MemberTypes types = MemberTypes.Property | MemberTypes.Field,
            BindingFlags bind = BindingFlags.Instance | BindingFlags.Public)
        {
            if (obj == null)
                return new List<MemberInfo>();

            return obj.GetType().GetMembers(bind).Where(
                m => types.HasFlag(m.MemberType));
        }


        /// <summary>
        /// Only returns members with a particular attribute
        /// </summary>
        /// <param name="includeInherited">I'm not quite sure</param>
        /// <returns>The filtered list of members</returns>
        public static IEnumerable<MemberInfo> FilterAttributeType<T>(
            this IEnumerable<MemberInfo> members,
            bool includeInherited = true) where T : Attribute
        {
            return members.Where(m => (m.GetCustomAttributes(
                typeof(T), includeInherited).Count() > 0));
        }


        public static IEnumerable<MemberInfo> RemoveReadOnly(
            this IEnumerable<MemberInfo> members)
        {
            foreach (var member in members) {
                if (member is FieldInfo) {
                    yield return member;
                } else if (member is PropertyInfo) {
                    if (((PropertyInfo)member).CanWrite)
                        yield return member;
                }
                // Constructors & methods can't be assigned to
                // I don't think events can be assigned to either
            }
        }
        public static IEnumerable<string> Names(
            this IEnumerable<MemberInfo> members)
        {
            return members.Select(m => m.Name).Distinct();
        }

        /// <summary>
        /// Returns a dictionary with the member names and their types
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Dictionary<string, Type> Declarations(
            this IEnumerable<MemberInfo> members)
        {
            var dict = new Dictionary<string, Type>();
            foreach (var member in members) {
                dict.Add(member.Name, member.ReturnType());
            }
            return dict;
        }

        /// <summary>
        /// Gets the return type of FieldInfo and PropertyInfo objects
        /// </summary>
        /// <param name="member">field or property</param>
        /// <returns>return type of member</returns>
        /// <exception cref="ArgumentException">Thrown if object isn't a
        /// FieldInfo or PropertyInfo object.</exception>
        public static Type ReturnType(this MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
            {
                return field.FieldType;
            }
            var property = member as PropertyInfo;
            if (property != null)
            {
                return property.PropertyType;
            }
            throw new ArgumentException(
                "Only fields and properties are suppored members");
        }

        /// <summary>
        /// See if a member looks and acts like a field. In other words,
        /// it has a single value that can be gotten and set.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>true if member is field-like</returns>
        public static bool Fieldlike(this MemberInfo member)
        {
            // if it is a fieldinfo, then of course it's fieldlike
            if (member is FieldInfo)
            {
                return true;
            }
            // otherwise make sure it's not an indexed property
            var property = member as PropertyInfo;
            return property.GetIndexParameters().Count() == 0;
        }


        /// <summary>
        /// Converts the object to a collection of key-value pairs
        /// </summary>        
        public static Dictionary<string, object> _AsDictionary(this object source,
            bool includeFields = true, bool includeProperties = true,
            Type attributeType = null, bool includeInheritedAttributes = true,
            bool includeReadOnlyProperties = true)
        {
            var include = MemberTypes.Field | MemberTypes.Property;
            if (!includeFields)
                include &= ~MemberTypes.Field;
            if (!includeProperties)
                include &= ~MemberTypes.Property;

            var members = source._GetMembers(include).Where(m => m.Fieldlike());

            if (attributeType != null) {
                var mi = typeof(ReflectionExtensions).GetMethod("FilterAttributeType");
                var filterRef = mi.MakeGenericMethod(attributeType);
                var args = new object[] { includeInheritedAttributes };
                filterRef.Invoke(members, args);
            }
            if (!includeReadOnlyProperties)
                members = members.RemoveReadOnly();
            return _AsDictionary(source, members.Names());
        }

        public static Dictionary<string, object> _AsDictionary(this object source, IEnumerable<string> fieldNames)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            foreach (var field in fieldNames)
                res.Add(field, source.GetFieldValue(field));
            return res;
        }

        public static T _ToObject<T>(this Dictionary<string, object> dict, ObjectReader reader = null)
        {
            if (reader == null) reader = new ObjectReader();
            if (dict == null || dict.Count == 0)
                return default(T);
            return (T)reader.Read(Activator.CreateInstance(typeof(T)), dict);
        }

        /// <summary>
        /// Sets the property or field on the object to the supplied value
        /// (converting the type if necessary)
        /// </summary>
        /// <remarks>throws FieldNotFoundException if the field can't be found</remarks>
        public static T _Set<T>(this T source, string fieldName, object value) where T : class // see Footnote 1 for why this where is here
        {
            Type type = source.GetType();

            var property = type.GetProperty(fieldName);

            object converted = null;

            if (property != null) {
                if (value.CanConvertTo(property.PropertyType, out converted))
                    property.SetValue(source, converted, null);
                else
                    throw new ConversionException("Error setting property '" + fieldName + "' - cannot convert to type '" + property.PropertyType.Name + "' from value '"
                        + (value == null ? "<null>" : value.ToString()) + "'");
            } else { // Try a field
                var field = type.GetField(fieldName);
                if (field != null)
                    if (value.CanConvertTo(field.FieldType, out converted))
                        field.SetValue(source, converted);
                    else
                        throw new ConversionException("Error setting field '" + fieldName + "' - cannot convert to type '" + field.FieldType.Name + "' from value '" +
                        (value == null ? "<null>" : value.ToString()) + "'");


                else {
                    throw new FieldNotFoundException(
                        "Missing field '" + fieldName + "' in " + source.Print());
                }
            }

            return source;


            // Footnote 1
            // You CANNOT use this to set properties on structs - hence the where T: class constraint
            // Normally, when you call this function with an object (anything that is defined with the "class" keyword, everything
            // works fine. Objects are always called with pass-by-reference semantics, so when you use reflection to set the propery/field,
            // it actually modifies the same object. HOWEVER, structs (or any other value type) have pass-by-value semantics, so 
            // every time you pass the thing to a function, it's getting a copy, not the original. Now, normally, this isn't a problem - this
            // is just the defined behavior for structs, so nothing suprising there. However, the Set, SetFrom, etc, methods also return
            // the thing they were passed (so you can chain them together), so even by pass-by-value semantics, you'd expect that the struct
            // you got back from this function would be the altered one (even if the original one didn't change).
            // For example, this should work:
            // var a = new MyStruct()._Set("Name", "foosums");
            // a.Name == "foosums" // should be true
            // This is where the problem comes in. The call to field.SetValue/property.SetValue also operates on pass-by-value sematics, 
            // so you end up making a copy of your struct, modifying it's value, and then throwing it away. 

            // The only way around this is to convert to a reference type before calling Set. For example:
            // http://stackoverflow.com/questions/448158/reflection-on-structure-differs-from-class-but-only-in-code

        }

        /// <summary>
        /// Update the fields on source from data (if they have the same names), converting the 
        /// types of objects if necessary
        /// </summary>
        /// <param name="except">Fields to exclude from the copy</param>
        /// <param name="caseSensitive">Are the field/property names case sensitive?</param>
        /// <returns>The original object (source)</returns>
        public static T _SetFrom<T>(this T source, object data, bool caseSensitive = true, IEnumerable<string> fields = null) where T : class
        {
            // We can't have a default parameter be an expression, so convert it from null to soemthing useful here
            if (fields == null)
                fields = data._GetMembers().RemoveReadOnly().Names();
            // Filter down to just those fields that were included

            // If it's already a Dict<string,object>, then just filter down to the keys we want
            // I'm not sure this is actually the best behaviour, it makes more sense just to
            // require a non-dictionary.
            var dataDict = data as Dictionary<string, object>;
            if (dataDict != null)
            {
                dataDict = dataDict.WhereKeys(f => fields.Contains(f));
            } else {
                dataDict = data._AsDictionary();
            }

            return _SetFrom(source, dataDict, caseSensitive);
        }

        /// <summary>
        /// Update the fields on source from data, converting the types if necessary
        /// </summary>
        /// <param name="caseSensitive">Are the field/property names case sensitive?</param>
        /// <returns>The original object (source)</returns>
        public static T _SetFrom<T>(this T source, Dictionary<string, object> data, bool caseSensitive = true, IEnumerable<string> fields = null) where T : class
        {
            // // We can't have a default parameter be an expression, so convert it from null to soemthing useful here
            if (fields == null)
                fields = data.Keys;

            Func<string, string> f;
            if (caseSensitive)
                f = s => s;
            else
                f = s => s.ToLower();

            // Filter down to just the fields we were told to include
            if (caseSensitive)
                data = data.WhereKeys(k => fields.Contains(k));
            else {
                // Use a lower-case comparison
                fields = fields.Select(s => f(s));
                data = data.WhereKeys(k => fields.Contains(f(k)));
            }

            return _SetFrom(source, data, f);
        }

        /// <summary>
        /// Update the fields on source from data, converting the types if necessary
        /// </summary>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <param name="fieldNameScrubbingFn">A function that is applied to the fieldnames of source and data to clean them up.
        /// If the cleaned data and source names match, the corresponding field will be set from data</param>
        /// <returns>The original object (source)</returns>
        public static T _SetFrom<T>(this T source, Dictionary<string, object> data, Func<string, string> fieldNameScrubbingFn) where T : class
        {
            // Create dictionaries of lowered fieldnames to their equivalents
            var sourceFields = source._GetMembers().RemoveReadOnly().Names().Distinct().IndexFlat(fieldNameScrubbingFn);
            var dataFields = data.Keys.IndexFlat(fieldNameScrubbingFn);
            foreach (var field in dataFields.Keys.Intersect(sourceFields.Keys))
                source._Set(sourceFields[field], data[dataFields[field]]);

            return source;
        }

        /// <summary>
        /// Are the two objects identical when compared over the supplied fields
        /// (if the second object defines more fields than the first, they will be ignored)
        /// </summary>
        public static bool _IsIdenticalOnFields(this object source, object other,
            IEnumerable<string> fieldNames = null)
        {
            if (fieldNames == null) fieldNames = source._GetMembers().Names();
            return source._AsDictionary(true).IsSameAs(other._AsDictionary(true), fieldNames);
        }

        /// <summary>
        /// Are the two objects identical when compared over the supplied fields
        /// </summary>
        public static bool _IsIdenticalOnFields(this object source, object other, params string[] fieldNames)
        {
            return _IsIdenticalOnFields(source, other, fieldNames as IEnumerable<string>);
        }
        public static string _Inspect(this object source, params string[] fieldNames)
        {
            return _Inspect(source, fieldNames as IEnumerable<string>);
        }
        public static string _Inspect(this object source, IEnumerable<string> fieldNames = null)
        {
            if (fieldNames == null)
                fieldNames = source._GetMembers().Names();
            var dict = source._AsDictionary(fieldNames);
            return dict.Print();
        }

        /// <summary>
        /// If source is a Func{object}, evaluate the function and return the result
        /// Otherwise, return just the object
        /// </summary>
        public static object Realize(this object source)
        {
            var func = source as Func<object>;
            return func == null ? source : func();
        }
    }
}
