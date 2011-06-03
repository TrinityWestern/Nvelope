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
                + "(?<areaBlock>\\(?(?<area>[\\d]{1,4})\\)?([\\s\\.\\-]+))?"
                + "(?<localBlock>(?<local>\\d[\\s\\d\\-\\.]{4,16}\\d))"
                + "(?<extensionBlock>\\s*(x|ext([\\.:]))?\\s*(?<extension>[\\d]{1,10})? *)?\\s*$");

        public string Country = "";
        public string Area = "";
        public string Local = "";
        public string Extension = "";

        public PhoneNumber(string str)
        {
            var match = PhoneNumber.StringFormat.Match(str);
            if (!match.Success)
                throw new ArgumentOutOfRangeException(
                    "'" + str + "' is not a parseable phone number.");
            this.Country = match.Groups["country"].Value;
            this.Area = match.Groups["area"].Value;
            this.Local = match.Groups["local"].Value;
            this.Extension = match.Groups["extension"].Value;
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
            if (PhoneNumber.StringFormat.Match(str).Success) {
                return new PhoneNumber(str);
            }
            return new PhoneNumber() { Local = str };
        }

        /// <summary>
        /// This is a safe method that tries to format a string as a phone
        /// number but just returns the original string on failure.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TryFormat(string str)
        {
            PhoneNumber phone;
            if (str.CanConvertTo<PhoneNumber>(out phone))
                return phone.ToString();
            return str;
        }

        public override string ToString() {
            string result = this.Local;
            if (this.Area != "")      result = this.Area + "-" + result;
            if (this.Country != "")   result = this.Country + "-" + result;
            if (this.Extension != "") result += "x" + this.Extension;
            return result;
        }

    }
}
