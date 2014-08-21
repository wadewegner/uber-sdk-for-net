using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Uber.Models;

namespace Uber.FunctionalTests
{
    [TestFixture]
    public class Tests
    {
        private float latitude = 37.5F;
        private float longitude = -122.2F;
        private string serverKey = "8_OWKYgAyaq_cnZgxeEIhk24cSLOYiYRrPMxRZvA";

        [Test]
        public async Task ResponseHeaders_NotNull()
        {
            var client = new UberClient(serverKey);
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
            var client = new UberClient(serverKey);
            var results = await client.ProductsAsync(latitude, longitude);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task PriceEstimates()
        {
            var client = new UberClient(serverKey);
            var results = await client.PriceEstimateAsync(latitude, longitude, latitude + 0.3F, longitude - 0.3F);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates()
        {
            var client = new UberClient(serverKey);
            var results = await client.TimeEstimateAsync(latitude, longitude);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates_Product()
        {
            var client = new UberClient(serverKey);
            var productResults = await client.ProductsAsync(latitude, longitude);
            var product = productResults.products.FirstOrDefault();

            if (product != null)
            {
                var results = await client.TimeEstimateAsync(latitude, longitude, productId: product.product_id);
                Assert.IsNotNull(results);
            }
        }
    }
}
