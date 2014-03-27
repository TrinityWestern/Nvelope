using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nvelope.Web
{
    public static class UriExtensions
    {
        /// <summary>
        /// Adds parameter to a uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="key">Key of the parameter to add.</param>
        /// <param name="value">Value of the parameter to add.</param>
        /// <returns>Uri with added parameter.</returns>
        public static Uri AddParameter(this Uri uri, string key, string value)
        {
            // Get current parameters
            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            // Add new parameter
            query[key] = value;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Output URI as a string without a slash at the end of the path
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>URI without ending slash</returns>
        public static string ToStringWithoutSlash(this Uri uri)
        {
            var cleanUri = uri.ToStringN();

            // Remove trailing slash
            cleanUri = cleanUri.TrimEnd('/');

            // Remove slash before query string
            cleanUri = cleanUri.Replace("/?", "?");

            // Remove slash before fragment
            cleanUri = cleanUri.Replace("/#", "#");

            return cleanUri;
        }
    }
}
