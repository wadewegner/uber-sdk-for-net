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
using NHtmlUnit.Html;
using NUnit.Framework;
using Uber.Models;

using NHtmlUnit;
using NHtmlUnit.Html;
using BrowserVersion = NHtmlUnit.BrowserVersion;
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
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };

            var url = Common.FormatAuthorizeUrl(ResponseTypes.Code, _clientId, HttpUtility.UrlEncode(_callbackUrl));
            var webClient = new WebClient(BrowserVersion.CHROME);

            webClient.Options.ThrowExceptionOnFailingStatusCode = false;
            webClient.Options.ThrowExceptionOnScriptError = false;
            webClient.Options.JavaScriptEnabled = true;
            webClient.Options.RedirectEnabled = true;
            webClient.Options.UseInsecureSsl = true;
            webClient.WaitForBackgroundJavaScript(2000);

            var page = webClient.GetHtmlPage(url);
            Assert.IsNotNull(page);

            var emailInputElement = (HtmlTextInput)page.GetElementByName("email");
            emailInputElement.Type(_username);

            var passwordInputElement = (HtmlPasswordInput)page.GetElementByName("password");
            passwordInputElement.Type(_password);

            var submitPage1Button = (HtmlSubmitInput)page.GetElementsByTagName("input")[4];
            Assert.IsNotNull(submitPage1Button);

            var page2 = (HtmlPage)submitPage1Button.Click();
            Assert.IsNotNull(page2);

            var submitPage2Button = (HtmlSubmitInput)page2.GetElementsByTagName("input")[3];
            Assert.IsNotNull(submitPage2Button);

            var page3 = submitPage2Button.Click();
            var page3Url = page3.Url.toString();
            Assert.IsNotNull(page3Url);

            var queryCollection = HttpUtility.ParseQueryString(page3Url);
            var code = queryCollection[0];
            Assert.IsNotNull(code);

            var auth = new AuthenticationClient();
            await auth.WebServerAsync(_clientId, _clientSecret, _callbackUrl, code);

            var apiVersion = auth.ApiVersion;
            var accessToken = auth.AccessToken;
            var refreshToken = auth.RefreshToken;

            Assert.IsNotNull(apiVersion);
            Assert.IsNotNull(accessToken);
            Assert.IsNotNull(refreshToken);
        }
    }
}
