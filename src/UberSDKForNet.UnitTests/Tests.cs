using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uber;

namespace UberSDKForNet.UnitTests
{
    [TestFixture]
    public class Tests
    {
        private UberClient _uberClient;

        [TestFixtureSetUp]
        public void Init()
        {
            _uberClient = new UberClient("");
        }

        [Test]
        public void UserActivity_Fail_ServerToken()
        {
            Assert.That(async () => await _uberClient.UserActivityAsync(), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public async Task WebServer_Fail_Arguments()
        {
            var auth = new AuthenticationClient();

            Assert.That(async () => await auth.WebServerAsync("", "", "", ""), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(async () => await auth.WebServerAsync("clientid", "", "", ""), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(async () => await auth.WebServerAsync("clientid", "clientSecret", "", ""), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(async () => await auth.WebServerAsync("clientid", "clientSecret", "redirectUri", ""), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(async () => await auth.WebServerAsync("clientid", "clientSecret", "redirectUri", "code"), Throws.InstanceOf<ArgumentException>());
        }
    }
}
