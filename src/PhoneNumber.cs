﻿using System;
using System.Text.RegularExpressions;

namespace Nvelope
{
    /// <remark>
    /// Some of the stuff in here might be locale specific
    /// </remark>
    public class PhoneNumber
    {
        public static Regex StringFormat = new Regex("^\\s*(?<countryBlock> *(?<country>[\\d]{1,4})([ \\.\\-]+))?"
                + "(?<areaBlock>(?<area>[\\d]{1,4})([ \\.\\-]+))?"
                + "(?<localBlock>(?<local>[\\d\\-\\.]{6,16}))"
                + "(?<extensionBlock>\\s*(x|ext([\\.:]))? *(?<extension>[\\d]{1,10})? *)?\\s*$");

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

        public string Country = "";
        public string Area = "";
        public string Local = "";
        public string Extension = "";

        public PhoneNumber(string str)
        {
            // this is a bad hack for now this stuff should be in the regex
            str = str.Replace("(", "").Replace(")", "").Replace("+", "");
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
        public PhoneNumber() {}

        public override string ToString() {
            string result = this.Local;
            if (this.Area != "")      result = this.Area + "-" + result;
            if (this.Country != "")   result = this.Country + "-" + result;
            if (this.Extension != "") result += "x" + this.Extension;
            return result;
        }

    }
}
