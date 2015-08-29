using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Web;
using ikvm.extensions;
using javax.tools;
using NHtmlUnit;
using NHtmlUnit.Html;
using NHtmlUnit.Javascript.Host;
using Uber.Models;
using WebClient = NHtmlUnit.WebClient;
using NUnit.Framework;
using Uber;

namespace Uber.FunctionalTests
{
    [TestFixture]
    public class Tests
    {
        private string _clientId = ConfigurationSettings.AppSettings["ClientId"];
        private string _clientSecret = ConfigurationSettings.AppSettings["ClientSecret"];
        private string _callbackUrl = ConfigurationSettings.AppSettings["CallbackUrl"];
        private string _userName = ConfigurationSettings.AppSettings["Username"];
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
                _userName = Environment.GetEnvironmentVariable("Username");
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
        public void CheckWebAuthFlow()
        {
            var url = Common.FormatAuthorizeUrl(ResponseTypes.Code, _clientId, HttpUtility.UrlEncode(_callbackUrl));
            var expectedUrl = string.Format("https://login.uber.com/oauth/authorize?response_type=code&client_id={0}&redirect_uri={1}", _clientId, HttpUtility.UrlEncode(_callbackUrl));

            Assert.AreEqual(url, expectedUrl);

            var webClient = new WebClient(BrowserVersion.FIREFOX_17);

            webClient.Options.ThrowExceptionOnScriptError = false;

            webClient.Options.JavaScriptEnabled = true;
            webClient.Options.RedirectEnabled = true;
            webClient.Options.ActiveXNative = false;
            webClient.Options.CssEnabled = true; 

            // ########################################
            // # Uber login page
            // ########################################

            var loginPage = webClient.GetHtmlPage(url);
            Assert.IsNotNull(loginPage);

            var loginPageText = loginPage.AsText();

            StringAssert.Contains("sign in", loginPageText.toLowerCase());
            StringAssert.Contains("connect with facebook", loginPageText.toLowerCase());
            StringAssert.Contains("don't have an account", loginPageText.toLowerCase());
            StringAssert.Contains("email", loginPageText.toLowerCase());
            StringAssert.Contains("password", loginPageText.toLowerCase());

            var signInSpan = (HtmlSpan)loginPage.GetElementsByTagName("span")[0];
            Assert.AreEqual(signInSpan.AsText(), "Sign In");

            var emailInputElement = (HtmlTextInput)loginPage.GetElementByName("email");
            Assert.IsNotNull(emailInputElement, "email");

            emailInputElement.SetValueAttribute(_userName);
            Assert.AreEqual(emailInputElement.Text, _userName);

            var passwordInputElement = (HtmlPasswordInput)loginPage.GetElementByName("password");
            Assert.IsNotNull(passwordInputElement, "password");

            passwordInputElement.SetValueAttribute(_password);
            Assert.AreEqual(passwordInputElement.Text, _password);

            var loginForm = (HtmlForm)loginPage.GetElementsByTagName("form")[0];
            Assert.IsNotNull(loginForm);

            var loginFormText = loginForm.AsText();

            StringAssert.Contains("sign in", loginFormText.toLowerCase());
            StringAssert.Contains("connect with facebook", loginFormText.toLowerCase());
            StringAssert.Contains("email", loginFormText.toLowerCase());
            StringAssert.Contains("password", loginFormText.toLowerCase());

            var loginFormButtons = loginForm.GetElementsByTagName("button");
            Assert.IsNotNull(loginFormButtons);

            var loginButton = (HtmlButton)loginFormButtons.First();
            Assert.IsNotNull(loginButton);

            var loginButtonText = loginButton.AsText();
            StringAssert.Contains("sign in", loginButtonText.toLowerCase());

            // ########################################
            // # Uber consent page
            // ########################################

            var consentPage = (HtmlPage)loginButton.Click();
            Assert.IsNotNull(consentPage);

            IPage callbackPage;

            // check if previously consented
            if (!consentPage.Url.ToString().contains(string.Format("{0}?code=", _callbackUrl)))
            {
                var consentPageUrl = consentPage.Url.ToString();
                Assert.IsNotNullOrEmpty(consentPageUrl);

                var consentPageText = consentPage.AsText();
                StringAssert.Contains("access to your full name", consentPageText.toLowerCase());

                var consentPageButtons = consentPage.GetElementsByTagName("button");
                Assert.IsNotNull(consentPageButtons);

                var noAllowButton = (HtmlButton)consentPageButtons[0];
                Assert.IsNotNull(noAllowButton);
                Assert.AreEqual("Deny", noAllowButton.AsText());

                var allowButton = (HtmlButton)consentPageButtons[1];
                Assert.IsNotNull(allowButton);
                Assert.AreEqual("Allow", allowButton.AsText());

                callbackPage = allowButton.Click();
                Assert.IsNotNull(callbackPage);
            }
            else
            {
                callbackPage = consentPage;
            }

            // ########################################
            // # Callback page
            // ########################################
            
            var page3Url = callbackPage.Url.ToString();
            Assert.IsNotNullOrEmpty(page3Url);
            StringAssert.Contains(string.Format("{0}?code=", _callbackUrl), page3Url);

            var queryCollection = HttpUtility.ParseQueryString(page3Url);
            var code = queryCollection[0];

            var auth = new AuthenticationClient();
            auth.WebServerAsync(_clientId, _clientSecret, _callbackUrl, code).Wait();

            Assert.IsNotNull(auth);
            Assert.IsNotNullOrEmpty(auth.AccessToken);
        }
    }
}
