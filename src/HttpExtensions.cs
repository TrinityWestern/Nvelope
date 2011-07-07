using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;

namespace Nvelope
{
    //TODO: The code in here should be moved into chocolate
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Retrieves a paramater from the GET or POST query that must be set.
        /// 
        /// If the parameter is not set properly, it will raise an HttpException
        /// </summary>
        /// <example>OptionalParam&lt;int&gt;(req, "userid")</example>
        /// <typeparam name="T">paramater type</typeparam>
        /// <param name="req">HttpRequest object</param>
        /// <param name="name">paramater name</param>
        /// <returns>instance of T</returns>
        public static T RequiredParam<T>(this HttpRequestBase req, string name)
        {
            // TODO: should result in a 404 or something like that, but we can't do
            // that yet. At the moment it just throws a FormatException
            if (!req.Params.ContainsKey(name))
                throw new HttpException("Required '" + name + "' not set");
            T output = default(T);
            if (req.Params[name].CanConvertTo<T>(out output))
                return output;
            else
                throw new HttpException("Parameter '" + name + "' is not the right format");
        }

        /// <summary>
        /// Retrieves a paramater from the GET or POST query that may no be set.
        /// 
        /// If the parameter is not set it will return the default_value. Throws an
        /// HttpException if paramater is set won't convert.
        /// </summary>
        /// <example>OptionalParam&lt;Datetime&gt;(req, "time", Datetime.Now)</example>
        /// <typeparam name="T">paramater type</typeparam>
        /// <param name="req">HttpRequest object</param>
        /// <param name="name">paramater name</param>
        /// <param name="default_value">value to return when not set</param>
        /// <returns>instance of T</returns>
        public static T OptionalParam<T>(this HttpRequestBase req, string name, T default_value)
        {
            if (!req.Params.ContainsKey(name)) return default_value;
            return req.RequiredParam<T>(name);
        }

        /// <summary>
        /// Checks to see if if a parameter exists
        /// </summary>
        /// <remarks>OptionalParam&lt;bool&gt; is better in some cases.</remarks>
        public static bool HasParam(this HttpRequestBase req, string name)
        {
            return req.Params.ContainsKey(name);
        }

        /// <summary>
        /// Filter HTTP parameters with a user defined function
        /// </summary>
        /// <example>Request.HandleParam("foo", HttpRequestHandlers.CommaList)</example>
        /// <typeparam name="T">paramater type</typeparam>
        /// <param name="req">HttpRequest object</param>
        /// <param name="name">paramater name</param>
        /// <param name="handle">user defined function to apply</param>
        /// <returns>instance of T</returns>
        public static T HandleParam<T>(this HttpRequestBase req, string name, Func<string, T> handle)
        {
            return handle(req.Params[name]);
        }

        /// <summary>
        /// returns a list of values from a http parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<T> ParamList<T>(this HttpRequestBase req, string name)
        {
            var stringvalue = req.RequiredParam<string>(name);
            return stringvalue.Split(",").Select(s => s.ConvertTo<T>());
        }

        /// <summary>
        /// Retrieves a boolean value for the checked status of a checkbox
        /// </summary>
        /// <param name="input">The HTML value of a checkbox</param>
        /// <returns>True when the Checkbox value is found in request.Params, False otherwise.</returns>
        public static bool CheckboxParam(this HttpRequestBase req, string name)
        {
            return req.Params.ContainsKey(name);
        }

        #region HttpRequest compatibility

        public static T RequiredParam<T>(this HttpRequest req, string name)
        {
            return new HttpRequestWrapper(req).RequiredParam<T>(name);
        }
        public static T OptionalParam<T>(this HttpRequest req, string name, T default_value)
        {
            return new HttpRequestWrapper(req).OptionalParam<T>(name, default_value);
        }
        public static bool HasParam(this HttpRequest req, string name)
        {
            return new HttpRequestWrapper(req).HasParam(name);
        }
        public static T HandleParam<T>(this HttpRequest req, string name, Func<string, T> handle)
        {
            return new HttpRequestWrapper(req).HandleParam<T>(name, handle);
        }
        public static IEnumerable<T> ParamList<T>(this HttpRequest req, string name)
        {
            return new HttpRequestWrapper(req).ParamList<T>(name);
        }
        public static bool CheckboxParam(this HttpRequest req, string name)
        {
            return new HttpRequestWrapper(req).CheckboxParam(name);
        }

        #endregion


    }
    /// <summary>
    /// User defined functions, built to work with HttpRequestExtensions.HandleParam
    /// 
    /// These functions should be static and take one argument of type string.
    /// </summary>
    public static class HttpRequestHandlers
    {
        /// <summary>
        /// Parameters separated by a ','
        /// </summary>
        /// <example>CommaList("1,2,3") == ["1", "2", "3"]</example>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete ("Use ParamList instead")]
        public static IEnumerable<string> CommaList(string input)
        {
            var result = new List<string>();
            if (input != null)
                result.AddRange(input.Split(',').Select(s => s.Trim()));
            return result;
        }
    }
}
