using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uber;

namespace Uber.UnitTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void UserActivity_Fail_ServerToken()
        {
            var client = new UberClient("");
            Assert.That(async () => await client.UserActivityAsync(), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void WebServer_Fail_Arguments()
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
