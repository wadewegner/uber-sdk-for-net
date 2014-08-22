using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using com.gargoylesoftware.htmlunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHtmlUnit.Html;
using NUnit.Framework;
using Uber.Models;

using NHtmlUnit;
using NHtmlUnit.Html;
using BrowserVersion = NHtmlUnit.BrowserVersion;
using HttpMethod = System.Net.Http.HttpMethod;
using WebClient = NHtmlUnit.WebClient;

namespace Uber.FunctionalTests
{
    [TestFixture]
    public class Tests
    {
        private string _clientId = ConfigurationSettings.AppSettings["ClientId"];
        private string _clientSecret = ConfigurationSettings.AppSettings["ClientSecret"];
        private string _callbackUrl = ConfigurationSettings.AppSettings["CallbackUrl"];
        private string _username = ConfigurationSettings.AppSettings["Username"];
        private string _password = ConfigurationSettings.AppSettings["Password"];
        private string _serverKey = ConfigurationSettings.AppSettings["ServerKey"];

        private float latitude = 37.5F;
        private float longitude = -122.2F;

        [TestFixtureSetUp]
        public void Init()
        {
            if (string.IsNullOrEmpty(_clientId))
            {
                _clientId = Environment.GetEnvironmentVariable("ClientId");
                _clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
                _callbackUrl = Environment.GetEnvironmentVariable("CallbackUrl");
                _username = Environment.GetEnvironmentVariable("Username");
                _password = Environment.GetEnvironmentVariable("Password");
                _serverKey = Environment.GetEnvironmentVariable("ServerKey");
            }
        }

        [Test]
        public async Task ResponseHeaders_NotNull()
        {
            var client = new UberClient(_serverKey);
            await client.ProductsAsync(latitude, longitude);

            Assert.IsNotNull(client.RateLimitRemaining);
            Assert.IsNotNull(client.Etag);
            Assert.IsNotNull(client.RateLimitReset);
            Assert.IsNotNull(client.RateLimitLimit);
            Assert.IsNotNull(client.UberApp);
        }

        [Test]
        public async Task Products()
        {
            var client = new UberClient(_serverKey);
            var results = await client.ProductsAsync(latitude, longitude);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task PriceEstimates()
        {
            var client = new UberClient(_serverKey);
            var results = await client.PriceEstimateAsync(latitude, longitude, latitude + 0.3F, longitude - 0.3F);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates()
        {
            var client = new UberClient(_serverKey);
            var results = await client.TimeEstimateAsync(latitude, longitude);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates_Product()
        {
            var client = new UberClient(_serverKey);
            var productResults = await client.ProductsAsync(latitude, longitude);
            var product = productResults.products.FirstOrDefault();

            if (product != null)
            {
                var results = await client.TimeEstimateAsync(latitude, longitude, productId: product.product_id);
                Assert.IsNotNull(results);
            }
        }

        [Test]
        public async Task Authentication_UserToken()
        {
            const string url = "http://oauthintsvc.cloudapp.net/api/oauth";

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };

                var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jObject = JObject.Parse(response);
                    var auth = JsonConvert.DeserializeObject<AuthToken>(jObject.ToString());

                    var accessToken = auth.access_token;
                    var refreshToken = auth.refresh_token;

                    Assert.IsNotNull(accessToken);
                    Assert.IsNotNull(refreshToken);
                }
            }
        }
    }
}
