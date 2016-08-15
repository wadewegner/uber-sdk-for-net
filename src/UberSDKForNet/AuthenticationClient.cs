using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uber.Models;
using Extensions;

namespace Uber
{
    public class AuthenticationClient
    {
        public string ApiVersion { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        private const string TokenUrl = "https://login.uber.com/oauth/token";
        private const string RevokeUrl = "https://login.uber.com/oauth/revoke";
        private string _apiVersion = "v1";
        private readonly HttpClient _httpClient;

        public AuthenticationClient()
            : this(new HttpClient())
        {
        }

        public AuthenticationClient(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _httpClient = httpClient;
            ApiVersion = "v1";
        }

        public string GetAuthorizeUrl(string clientId, string version = "v2", string redirect_uri = null, List<string> scopes = null)
        {
            return String.Format("https://login.uber.com/oauth/{0}/authorize?client_id={1}&response_type=code{2}{3}",
                version, clientId, String.IsNullOrEmpty(redirect_uri) ? "" : "&redirect_uri=" + redirect_uri,
                scopes == null ? "" : "&scopes=" + Common.Join(scopes, " "));
        }

        private KeyValuePair<string, string>[] _validateWebServerParameters(string clientId, string clientSecret, string redirectUri, string miscellaneousCode, 
            string grant_type = "authorization_code", List<KeyValuePair<string, string>> headers = null)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException("clientSecret");
            if (string.IsNullOrEmpty(redirectUri)) throw new ArgumentNullException("redirectUri");
            if (string.IsNullOrEmpty(miscellaneousCode)) throw new ArgumentNullException("code");
            if (!Common.IsValidUri(redirectUri)) throw new ArgumentException("Invalid redirectUri");

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", grant_type),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                };
            if (headers != null)
            {
                for (int i = 0; i < headers.Count; i++)
                {
                    content.Add(headers[i]);
                }
            }
            return content.ToArray();
        }

        private string _getFormParameterString(List<KeyValuePair<string, string>> parameters)
        {
            string ret = "";
            for (int i = 0; i < parameters.Count; i++)
            {
                ret += parameters[i].Key + "=" + parameters[i].Value;
                if (i < parameters.Count - 1) ret += "&";
            }
            return ret;
        }

        public Promise<AuthToken> GetAccessToken(string clientId, string clientSecret, string redirectUri, string code)
        {
            AuthToken ret = null;
            return Promise<AuthToken>.Create(() =>
            {
                TryGetAccessToken(clientId, clientSecret, redirectUri, code).Success((string response) =>
                {
                    ret = JsonConvert.DeserializeObject<AuthToken>(response);
                }).Wait();
                return ret;
            });
        }
        
        public Promise<string> TryGetAccessToken(string clientId, string clientSecret, string redirectUri, string code)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("code", code)
            };
            string grant_type = "authorization_code";
            KeyValuePair<string, string>[] content = _validateWebServerParameters(clientId, clientSecret, redirectUri, code, grant_type, headers);
            string requestContent = _getFormParameterString(content.ToList());
            return Api.PostAsync(TokenUrl, requestContent, "application/x-www-form-urlencoded", null, true);
        }

        public Promise<AuthToken> Refresh_Token(string clientId, string clientSecret, string redirectUri, string refreshToken)
        {
            AuthToken ret = null;
            return Promise<AuthToken>.Create(() =>
            {
                TryRefreshToken(clientId, clientSecret, redirectUri, refreshToken).Success((string response) =>
                {
                    ret = JsonConvert.DeserializeObject<AuthToken>(response);
                }).Wait();
                return ret;
            });
        }

        public Promise<string> TryRefreshToken(string clientId, string clientSecret, string redirectUri, string refreshToken)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };
            string grant_type = "refresh_token";
            KeyValuePair<string, string>[] content = _validateWebServerParameters(clientId, clientSecret, redirectUri, refreshToken, grant_type, headers);
            string requestContent = _getFormParameterString(content.ToList());
            return Api.PostAsync(TokenUrl, requestContent, "application/x-www-form-urlencoded", null, true);
        }
        
        public Promise<string> TryRevokeToken(string clientId, string clientSecret, string redirectUri, string accessToken)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("token", accessToken)
            };
            KeyValuePair<string, string>[] content = _validateWebServerParameters(clientId, clientSecret, redirectUri, accessToken, "revoke_token", headers);
            string requestContent = _getFormParameterString(content.ToList());
            return Api.PostAsync(RevokeUrl, requestContent, "application/x-www-form-urlencoded", null, true);
        }

        public async Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("code", code)
            };
            var content = new FormUrlEncodedContent(_validateWebServerParameters(clientId, clientSecret, redirectUri, code, "authorization_code", headers));
            
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(TokenUrl),
                Content = content
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var authToken = JsonConvert.DeserializeObject<AuthToken>(response);

                AccessToken = authToken.access_token;
                RefreshToken = authToken.refresh_token;
            }
            else
            {
                //TODO: Create appropriate error response
                //var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                //throw new ForceAuthException(errorResponse.error, errorResponse.error_description);
                throw new Exception(response);
            }
        }
    }
}
