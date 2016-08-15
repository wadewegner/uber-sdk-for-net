using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uber.Models;

namespace Uber
{
    public static class Common
    {
        private const string _authorizeUrl = "https://login.uber.com/oauth/authorize";

        public static string FormatAuthorizeUrl(
            ResponseTypes responseType,
            string clientId,
            string redirectUrl,
            string state = "",
            string scope = "")
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(redirectUrl)) throw new ArgumentNullException("redirectUrl");

            var url =
            string.Format(
                "{0}?response_type={1}&client_id={2}&redirect_uri={3}",
                _authorizeUrl,
                responseType.ToString().ToLower(),
                clientId,
                redirectUrl);

            if (!string.IsNullOrEmpty(state))
                url += string.Format("state={0}", state);

            if (!string.IsNullOrEmpty(scope))
                url += string.Format("scope={0}", scope);

            return url;
        }

        public static string FormatUrl(string instanceUrl, string apiVersion, string resourceName)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");

            return string.Format("{0}/{1}/{2}", instanceUrl, apiVersion, resourceName);
        }

        public static bool IsValidUri(string url)
        {
            Uri tempValue;
            return Uri.TryCreate(url, UriKind.Absolute, out tempValue);
        }

        public static string Join(List<string> arr, string concatenator = ",") {
            string ret = "";
            for (int i = 0; i < arr.Count; i++)
            {
                ret += arr[i];
                if (i < arr.Count - 1) ret += concatenator;
            }
            return ret;
        }
    }
}
