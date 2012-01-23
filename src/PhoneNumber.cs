using System;
using System.Text.RegularExpressions;

namespace Nvelope
{
    /// <remark>
    /// Some of the stuff in here might be locale specific
    /// </remark>
    public class PhoneNumber
    {
        public static Regex StringFormat = new Regex(
            "^\\s*(?<countryBlock>\\+?\\s?(?<country>[\\d]{1,4})([\\s\\(\\.\\-]+))?"
                + "[\\s\\.\\-]*" // this is incase the contry code is ommited but
                // there's still puntuation before the rest of the phone number
                + "(?<areaBlock>\\(?(?<area>[\\d]{1,4})([\\)\\s\\.\\-]+))?"
                + "(?<localBlock>(?<local>\\d[\\s\\d\\-\\.]{4,16}\\d))"
                + "\\s*(x|ext([\\.:]))?\\s*(?<extension>[\\d]{1,10})?\\s*$");

        public string Country = "";
        public string Area = "";
        public string Local = "";
        public string Extension = "";

        public PhoneNumber(string str)
        {
            var match = StringFormat.Match(str);
            if (!match.Success)
                throw new ArgumentOutOfRangeException(
                    "'" + str + "' is not a parseable phone number.");
            Country = match.Groups["country"].Value;
            Area = match.Groups["area"].Value;
            Local = match.Groups["local"].Value;
            Extension = match.Groups["extension"].Value;
        }

        /// <summary>
        /// A phone constructor where you set you own parts
        /// </summary>
        public PhoneNumber() { }

        /// <summary>
        /// Makes a phone number even if it's invalid.
        /// </summary>
        /// <returns>Invalid numbers just have the number stuffed in the
        /// Local portion</returns>
        public static PhoneNumber CreateAnyway(string str)
        {
            if (str == null) {
                return new PhoneNumber();
            }
            if (StringFormat.Match(str).Success) {
                return new PhoneNumber(str);
            }
            return new PhoneNumber { Local = str };
        }

        /// <summary>
        /// This is a safe method that tries to format a string as a phone
        /// number but just returns the original string on failure.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TryFormat(string str)
        {
            if (str == null) return str;
            var phone = str.ConvertAs<PhoneNumber>();
            return phone != null ? phone.ToString() : str;
        }

        /// <summary>
        /// This method tries to format a simple string of digits to
        /// match phone format
        /// This method is just used to display the phone number in correct format.
        /// It will return 0 if no
        /// </summary>
        /// <param name="str">alpha numeric string</param>
        /// <returns>string of integers formated like a phone number</returns>
        public static string FormatPhoneNumber(string str)
        {
            // Bit of a hack - remove any non-numeric characters
            str = Regex.Replace(str, "[^0-9]", "");
            int len = str.Length;
            if (len == 11)
            {
                return String.Format("{0}-{1}-{2}-{3}", 
                    str.Substring(0, 1), 
                    str.Substring(1, 3), 
                    str.Substring(4, 3), 
                    str.Substring(7, 4));
            }
            if (len == 10)
            {
                return String.Format("{0}-{1}-{2}",
                    str.Substring(0, 3),
                    str.Substring(3, 3),
                    str.Substring(6, 4));
            }
            if (len == 7)
            {
                return String.Format("{0}-{1}",
                    str.Substring(0, 3),
                    str.Substring(3, 4));
            }
            else
                return str;
        }

        public override string ToString() {
            var result = Local;
            if (!string.IsNullOrEmpty(Area))      result = Area + "-" + result;
            if (!string.IsNullOrEmpty(Country))   result = Country + "-" + result;
            if (!string.IsNullOrEmpty(Extension)) result += "x" + Extension;
            return result;
        }

        /// <summary>
        /// Returns a "normalized" form of phone number (essentially only the digits)
        /// </summary>
        /// <returns>Number as string (e.g. "(604)853-7994" becomes "6048537994")</returns>
        public string Normalized()
        {
            var concat = this.Country + this.Area + this.Local + this.Extension;
            return Regex.Replace(concat, "\\D", "");
        }
    }
}
