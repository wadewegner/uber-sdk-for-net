using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Uber.Models;

namespace Uber
{
    public class UberClient : ResponseHeader
    {
        private readonly string _url = "https://api.uber.com";
        private readonly string _apiVersion;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private TokenTypes _tokenType;

        public UberClient(string token)
            : this(TokenTypes.Server, token, "v1", new HttpClient())
        {
        }

        public UberClient(TokenTypes tokenType, string token)
            : this(tokenType, token, "v1", new HttpClient())
        {
        }

        public UberClient(string token, string apiVersion)
            : this(TokenTypes.Server, token, apiVersion, new HttpClient())
        {
        }

        public UberClient(TokenTypes tokenType, string token, string apiVersion, HttpClient httpClient)
        {
            _tokenType = tokenType;
            _token = token;
            _apiVersion = apiVersion;
            _httpClient = httpClient;

            if (_tokenType == TokenTypes.Server)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Products> ProductsAsync(float latitude, float longitude)
        {
            var urlSuffix = string.Format("products?latitude={0}&longitude={1}", latitude.ToString("R"), longitude.ToString("R"));
            var url = Common.FormatUrl(_url, _apiVersion, urlSuffix);

            return await HttpGetAsync<Products>(url);
        }

        public async Task<Prices> PriceEstimateAsync(float startLatitude, float startLongitude, float endLatitude, float endLongitude)
        {
            var urlSuffix = string.Format(
                "estimates/price?start_latitude={0}&start_longitude={1}&end_latitude={2}&end_longitude={3}", 
                startLatitude.ToString("R"), 
                startLongitude.ToString("R"), 
                endLatitude.ToString("R"), 
                endLongitude.ToString("R"));

            var url = Common.FormatUrl(_url, _apiVersion, urlSuffix);

            return await HttpGetAsync<Prices>(url);
        }

        public async Task<Times> TimeEstimateAsync(float startLatitude, float startLongitude, string customerUuid = "", string productId = "")
        {
            var urlSuffix = string.Format("estimates/time?start_latitude={0}&start_longitude={1}", startLatitude,
                startLongitude);

            if (!string.IsNullOrEmpty(customerUuid))
                urlSuffix += string.Format("&customer_uuid={0}", customerUuid);

            if (!string.IsNullOrEmpty(productId))
                urlSuffix += string.Format("&product_id={0}", productId);

            var url = Common.FormatUrl(_url, _apiVersion, urlSuffix);

            return await HttpGetAsync<Times>(url);
        }

        public async Task<UserActivity> UserActivityAsync(int offset = 0, int limit = 5)
        {
            if (_tokenType == TokenTypes.Server) throw new ArgumentException("Wrong token type! Use access token instead of server token.");

            var urlSuffix = string.Format("history?offset={0}&limit={1}", offset, limit);
            var url = Common.FormatUrl(_url, _apiVersion, urlSuffix);

            return await HttpGetAsync<UserActivity>(url);
        }

        public async Task<User> UserAsync()
        {
            if (_tokenType == TokenTypes.Server) throw new ArgumentException("Wrong token type! Use access token instead of server token.");

            var url = Common.FormatUrl(_url, _apiVersion, "me");

            return await HttpGetAsync<User>(url);
        }

        private async Task<T> HttpGetAsync<T>(string url)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                RateLimitRemaining = responseMessage.Headers.GetValues("X-Rate-Limit-Remaining").FirstOrDefault();
                Etag = responseMessage.Headers.GetValues("Etag").FirstOrDefault();
                RateLimitReset = responseMessage.Headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault();
                RateLimitLimit = responseMessage.Headers.GetValues("X-Rate-Limit-Limit").FirstOrDefault();
                UberApp = responseMessage.Headers.GetValues("X-Uber-App").FirstOrDefault();

                var jObject = JObject.Parse(response);
                var r = JsonConvert.DeserializeObject<T>(jObject.ToString());
                return r;
            }

            throw new Exception("error");
        }

    }
}
