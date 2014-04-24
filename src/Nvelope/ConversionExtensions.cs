using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Nvelope.Exceptions;

namespace Nvelope
{
    /// <summary>
    /// Extensions for conversions....
    /// </summary>
    public static class ConversionExtensions
    {
        /// <summary>
        /// Is the string convertible to a bool?
        /// </summary>
        public static bool IsBool(this string source)
        {
            if (source == null)
                return false;
            try {
                source.ToBoolFriendly();
                return true;
            } catch (ConversionException) {
                return false;
            }
        }

        /// <summary>
        /// Tries to convert string to bool
        /// </summary>
        /// <remarks>Throws a ConversionException if it can't convert - call IsBool to check</remarks>
        public static bool ToBoolFriendly(this string source)
        {
            switch (source.Trim().ToLower())
            {
                case ("true"):
                case ("1"):
                case ("t"):
                case ("y"):
                case ("yes"):
                case ("on"):
                    return true;
                case ("false"):
                case ("0"):
                case ("f"):
                case ("n"):
                case ("no"):
                case ("off"):
                    return false;
                default:
                    throw new ConversionException(
                        "Could not convert value '" + source + "' to a Boolean");
            }
        }

        /// <summary>
        /// Converts a two digit year into a 4 digit one
        /// The range will be the last 8 decades, the current one, and the next
        /// </summary>
        public static int MakeFourDigitYear(int twoDigitYear)
        {
            if (twoDigitYear > 99 || twoDigitYear < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "twoDigitYear",
                    "two digit year must be less than 100");
            }

            var now = DateTime.Now.Year;

            var curDecade = (now / 10) % 10;
            // ie, 20 for 2010
            var curCentury = now / 100 ;

            // Take 10 decades, where the first one is the current decade
            var decades = 0.To(9).Repeat().Skip(curDecade).Take(10);
            // A map of decade # -> century #
            var centuries = decades.MapIndex(i => 0);

            // Count through our decades, associating them with a century            
            foreach (var dec in decades)
            {
                centuries[dec] = curCentury;
                if (dec == 9) // If we're rolling around, inc century
                    curCentury++;
                if (dec == decades.Second()) // We're only counting 2 decades into the future, 
                    curCentury--;           // all the rest are in the past
            }

            // get the second digit
            var targetDecade = twoDigitYear % 100 / 10;
            return twoDigitYear + centuries[targetDecade] * 100;
        }

        /// <summary>
        /// Expects a date format of day/month/year
        /// </summary>
        public static DateTime? ToDateTimeNullable(this string source)
        {
            DateTime res = DateTime.MinValue;

            if (DateTime.TryParse(
                source,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out res))
            {
                return res;
            }

            var match = Regex.Match(source, "^ *(\\d{1,2})[\\\\/\\-](\\d{1,2})[\\\\/\\-](\\d{2,4})( +(\\d{1,2}) *: *(\\d{1,2})( *: *)?(\\d{1,2})?( *)?(AM|PM)?)? *$");
            if (match.Success)
            {
                int year, month, day, hour = 0, minute = 0, second = 0;
                // Check the first two elements - if one is greater than 12, it's the day, and the other is year
                // otherwise, default to dd/mm/yyyy
                var a = match.Groups[1].Value.ConvertTo<int>();
                var b = match.Groups[2].Value.ConvertTo<int>();
                if (a > 12)
                {
                    day = a;
                    month = b;
                }
                else
                {
                    day = b;
                    month = a;
                }

                // Group 3 is year
                year = match.Groups[3].Value.ConvertTo<int>();
                if (year < 100)
                    year = MakeFourDigitYear(year);


                // See if we've got an AM/PM indicator
                var isPM = match.Groups[10].Success ? match.Groups[10].Value == "PM" : false;
                // Get the hour - if it's PM, add 12
                if (match.Groups[5].Success)
                {
                    bool isNoon = match.Groups[5].Value == "12";
                    hour = match.Groups[5].Value.ConvertTo<int>() + ((isPM && !isNoon) ? 12 : 0);
                }

                if (match.Groups[6].Success)
                    minute = match.Groups[6].Value.ConvertTo<int>();

                // Seconds are optional
                if (match.Groups[7].Success)
                    second = match.Groups[8].Value.ConvertTo<int>();

                return new DateTime(year, month, day, hour, minute, second);
            }

            return null;
        }

        /// <summary>
        /// Try very hard to convert this string to a datetime
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ToDateTimeFriendly(this string source)
        {
            var res = ToDateTimeNullable(source);
            if (res == null)
            {
                throw new ConversionException(
                    "Could not convert value \"" + source + "\" to DateTime");
            }
            else
            {
                return res.Value;
            }

        }

        public static Month ToMonth(this string source)
        {
            switch (source.Trim().ToLower())
            {
                case "jan":
                case "january":
                    return Month.January;
                case "feb":
                case "february":
                    return Month.February;
                case "mar":
                case "march":
                    return Month.March;
                case "apr":
                case "april":
                    return Month.April;
                case "may":
                    return Month.May;
                case "jun":
                case "june":
                    return Month.June;
                case "jul":
                case "july":
                    return Month.July;
                case "aug":
                case "august":
                    return Month.August;
                case "sep":
                case "sept":
                case "september":
                    return Month.September;
                case "oct":
                case "october":
                    return Month.October;
                case "nov":
                case "november":
                    return Month.November;
                case "dec":
                case "december":
                    return Month.December;
                default:
                    throw new ConversionException("Could not convert '" + source + "' to Month");
            }
        }

        /// <summary>
        /// Is the string convertible to XML? If so, doc will hold the resultant document
        /// </summary>
        /// <param name="doc">The document, if loading was successful, else null</param>
        public static bool IsXml(this string source, out XmlDocument doc)
        {
            try {
                doc = ToXml(source);
                return true;
            } catch (XmlException) {
                doc = null;
                return false;
            }
        }

        /// <summary>
        /// Converts the string to an XMLDocument.
        /// </summary>
        /// <remarks>Throws an exception if the string isn't well-formed. If you're unsure, use
        /// IsXml to check</remarks>
        public static XmlDocument ToXml(this string xmlString)
        {
            // TODO: This method assumes that xmlString isn't actually valid XML, but doesn't properly escape it

            var doc = new XmlDocument();
            // Only replace & - if there's anything else malformed in the string, we're screwed
            doc.LoadXml(xmlString.ChopStart(Environment.NewLine).Replace("&", "&amp;"));
            return doc;
        }

        public static object TryEnum(Type type, object source)
        {
            if (!type.IsEnum)
                return null;

            object res;
            if (Enum.GetUnderlyingType(type) == typeof(int) && source.CanConvertTo<int>())
                res = Enum.ToObject(type, source.ConvertTo<int>());
            else
                if (type == typeof(Month))
                    return source.ToStringN().ToMonth();
                else
                    res = Enum.Parse(type, source.ToStringN());

            if (Enum.IsDefined(type, res))
                return res;
            else
                throw new ConversionException("Tried to convert '" + source.ToStringN() + "' to type '" + type.Name +
                            "', but there was no value in " + type.Name + " matching that value.");
            
        }

        /// <summary>
        /// Convert to another type intelligently
        /// </summary>
        /// <remarks>Does a better job than the build-in Convert.ChangeType</remarks>
        /// <exception cref="ConversionException">The conversion somehow failed.</exception>
        public static object ConvertTo(this object source, Type type)
        {
            if (type == null && (source == null || (source as string) == "NULL"))
                return null;

            Type sourceType = null;

            // Maybe it's already the correct type
            if (source != null)
            {
                sourceType = source.GetType();
                if (sourceType == type || sourceType.IsSubclassOf(type))
                    return source;
            }

            if (source == DBNull.Value)
                source = null;

            // If we're trying to convert to object, just return the thing
            if (type == typeof(object))
                return source;

            var sourceString = source as string;

            // Handle nullable types
            if (Reflection.ReflectionExtensions.IsNullable(type))
            {
                // If we're trying to set to null, perfect!
                if (source == null)
                    return source;

                // try to convert to the underlying base type instead
                type = type.GetGenericArguments()[0];

                if (sourceString != null)
                {
                    // If we're trying to convert to a nullable datetime, use the extension method
                    if (type == typeof(DateTime))
                        return sourceString.ToDateTimeNullable();

                    if (type == typeof(bool) && sourceString.IsBool())
                        return sourceString.ToBoolFriendly();

                    // If we're expecting a nullable type and we've got an empty string,
                    // return null
                    if (sourceString.IsNullOrEmpty())
                        return null;
                }
            }

            if (type == typeof(bool)) {
                if (source == null)
                    throw new ConversionException(
                        "Cannot convert null into a bool - if the value might be null, " + 
                        "check before calling ConvertTo, or call ConvertTo<bool?>() instead");
                else
                    return source.ToString().ToBoolFriendly();
            }

            var enumRes = TryEnum(type, source);
            if (enumRes != null)
                return enumRes;

            if (sourceString != null)
            {
                if (type == typeof(DateTime))
                {
                    return sourceString.ToDateTimeFriendly();
                }
                if (type == typeof(Month))
                    return sourceString.ToMonth();
                if (type.IsEnum)
                {
                    if (Enum.GetUnderlyingType(type) == typeof(int))
                    {
                        int number;
                        // If the result is numeric, convert to an int and cast to the enum
                        if (Int32.TryParse(sourceString, out number))
                            return number;
                    }

                    // Otherwise, try to parse the enum name
                    return Enum.Parse(type, sourceString);
                }
                else if (type == typeof(Guid))
                    return new Guid(sourceString);
                else if (type == typeof(XmlDocument))
                    return sourceString.ToXml();
                else if (type == typeof(decimal))
                {
                    // If it's in scientific notation, convert to double, then to decimal
                    if (Regex.IsMatch(sourceString, "^\\d+\\.\\d+E\\-?\\d+$"))
                        return sourceString.ConvertTo<double>().ConvertTo<decimal>();
                }
            }

            // If it's an int-based enumeration, then try to convert by converting
            // the int to the enum
            // NOTE: I don't think this will work with Flag enumerations, since they
            // might have values that aren't defined in the list - ie, they can be made up
            // of a combination of values |'ed together
            if (source is int && type.IsEnum && Enum.GetUnderlyingType(type) == typeof(int))
            {
                var i = (int)source;
                return Enum.ToObject(type, i);
            }

            // Handle parsing a byte array to a string
            if (type == typeof(string))
            {
                var sourceBytes = source as byte[];
                if (sourceBytes != null)
                {
                    return sourceBytes.ToUtf8String();
                }
            }

            try {
                // If nothing else works, let .NET try to do it
                return Convert.ChangeType(source, type, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException) { }
            catch (FormatException) { }
            catch (OverflowException) { }
            catch (ArgumentNullException) { }

            // If the type is assignable, we can just do a cast
            if (type.IsAssignableFrom(sourceType))
                return source;

            // Last ditch effort - look at the constructors for the class, and see if there's a 
            // constructor that takes our input type
            ConstructorInfo constructor = null;
            if (sourceType != null)
                constructor = type.GetConstructor(new Type[] { sourceType });

            if (constructor != null)
            {
                try
                {
                    return constructor.Invoke(new object[] { source });
                }
                catch (TargetInvocationException) { }
            }
            throw new ConversionException("Could not convert value '" +
                    source.ToStringN() + "' to requested type '" + type.Name + "'");
        }

        /// <summary>
        /// Convert to another type intelligently
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object source)
        {
            return (T)ConvertTo(source, typeof(T));
        }

        /// <summary>
        /// Can we convert this value to some other type?
        /// </summary>
        public static bool CanConvertTo(this object source, Type type)
        {
            try {
                ConvertTo(source, type);
                return true;
            }
            catch (ConversionException)
            {
                return false;
            }
        }

        /// <summary>
        /// Can we convert this value to some other type?
        /// </summary>
        public static bool CanConvertTo<T>(this object source)
        {
            try
            {
                ConvertTo<T>(source);
                return true;
            }
            catch (ConversionException)
            {
                // This is for if we're trying to convert from a string to an enum
                // and there is no enum value that corresponds to that string
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

        }

        /// <summary>
        /// Like ConvertTo, but returns null on failure instead of throwing
        /// an exception. Like the 'as' keyword.
        /// </summary>
        /// <remarks>Use ConvertNullable for value types</remarks>
        public static T ConvertAs<T>(this object source) where T : class
        {
            T result = null;
            try
            {
                result = ConvertTo<T>(source);
            }
            catch (ConversionException)
            {
            }
            return result;
        }

        /// <summary>
        /// Like ConvertTo, but returns null on failure instead of throwing
        /// an exception. Like the 'as' keyword.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertAs(this object source, Type type)
        {
            try
            {
                return ConvertTo(source, type);
            }
            catch (ConversionException)
            {
                return null;
            }
        }

        /// <summary>
        /// Like ConvertAs but for value types. Makes a nullable version of
        /// the value.
        /// </summary>
        public static T? ConvertNullable<T>(this object source) where T : struct
        {
            T? result = null;
            try
            {
                result = ConvertTo<T>(source);
            }
            catch (ConversionException)
            {
            }
            return result;
        }
    }
}
