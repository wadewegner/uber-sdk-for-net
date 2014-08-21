using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Uber;
using Uber.Models;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _clientId = ConfigurationSettings.AppSettings["CliendId"];
        private readonly string _clientSecret = ConfigurationSettings.AppSettings["ClientSecret"];
        private readonly string _callbackUrl = ConfigurationSettings.AppSettings["CallbackUrl"];

        public ActionResult Login()
        {
            var url =
                Common.FormatAuthorizeUrl(
                    ResponseTypes.Code,
                    _clientId,
                    HttpUtility.UrlEncode(_callbackUrl));

            return Redirect(url);
        }

        public async Task<ActionResult> Callback(string code)
        {
            var auth = new AuthenticationClient();
            await auth.WebServerAsync(_clientId, _clientSecret, _callbackUrl, code);

            Session["ApiVersion"] = auth.ApiVersion;
            Session["AccessToken"] = auth.AccessToken;
            Session["RefreshToken"] = auth.RefreshToken;

            var client = new UberClient(TokenTypes.Access, auth.AccessToken, "v1", new HttpClient());

            var userActivity = await client.UserActivityAsync();
            var user = await client.UserAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}