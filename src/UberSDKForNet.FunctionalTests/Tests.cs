using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Uber.FunctionalTests
{
    [TestFixture]
    public class Tests
    {
        private UberClient _uberClient;

        [TestFixtureSetUp]
        public void Init()
        {
            _uberClient = new UberClient("8_OWKYgAyaq_cnZgxeEIhk24cSLOYiYRrPMxRZvA");
        }

        [Test]
        public async Task Products()
        {
            var results = await _uberClient.ProductsAsync(37.7833F, -122.419416F); // San Francisco

            Assert.IsNotNull(results);
        }

        [Test]
        public async Task PriceEstimates()
        {
            var results = await _uberClient.PriceEstimateAsync(37.5F, -122.2F, 37.8F, -122.5F); // San Francisco

            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates()
        {
            var results = await _uberClient.TimeEstimateAsync(37.5F, -122.2F); // San Francisco

            Assert.IsNotNull(results);
        }

        [Test]
        public async Task TimeEstimates_Product()
        {
            var productResults = await _uberClient.ProductsAsync(37.7833F, -122.419416F); // San Francisco
            var product = productResults.products.FirstOrDefault();
            if (product != null)
            {
                var results = await _uberClient.TimeEstimateAsync(37.5F, -122.2F, productId: product.product_id); // San Francisco

                Assert.IsNotNull(results);
            }
        }


    }
}
