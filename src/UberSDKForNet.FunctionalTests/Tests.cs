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
        private UberClient _uberClient;
        private float latitude = 37.5F;
        private float longitude = -122.2F;

        [TestFixtureSetUp]
        public void Init()
        {
            _uberClient = new UberClient("8_OWKYgAyaq_cnZgxeEIhk24cSLOYiYRrPMxRZvA");
        }

        [Test]
        public async Task ResponseHeaders_NotNull()
        {
            await _uberClient.ProductsAsync(latitude, longitude);

            Assert.IsNotNull(_uberClient.RateLimitRemaining);
            Assert.IsNotNull(_uberClient.Etag);
            Assert.IsNotNull(_uberClient.RateLimitReset);
            Assert.IsNotNull(_uberClient.RateLimitLimit);
            Assert.IsNotNull(_uberClient.UberApp);
        }

        [Test]
        public async Task Products()
        {
            var results = await _uberClient.ProductsAsync(latitude, longitude);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task PriceEstimates()
        {
            var results = await _uberClient.PriceEstimateAsync(latitude, longitude, latitude + 0.3F, longitude - 0.3F);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates()
        {
            var results = await _uberClient.TimeEstimateAsync(latitude, longitude);
            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates_Product()
        {
            var productResults = await _uberClient.ProductsAsync(latitude, longitude);
            var product = productResults.products.FirstOrDefault();

            if (product != null)
            {
                var results = await _uberClient.TimeEstimateAsync(latitude, longitude, productId: product.product_id);
                Assert.IsNotNull(results);
            }
        }



    }
}
