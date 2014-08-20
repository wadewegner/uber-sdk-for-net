using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Uber.FunctionalTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task GetToken()
        {
            var uber = new UberClient("8_OWKYgAyaq_cnZgxeEIhk24cSLOYiYRrPMxRZvA");
            var results = await uber.ProductsAsync(37.7833F, -122.419416F);

            Assert.IsNotNull(results);
        }
    }
}
