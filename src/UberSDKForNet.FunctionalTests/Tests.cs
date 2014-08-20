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
        public async Task Products()
        {
            var uber = new UberClient("8_OWKYgAyaq_cnZgxeEIhk24cSLOYiYRrPMxRZvA");
            var results = await uber.ProductsAsync(37.7833F, -122.419416F); // San Francisco

            Assert.IsNotNull(results);
        }

        [Test]
        public async Task PriceEstimate()
        {
            var uber = new UberClient("8_OWKYgAyaq_cnZgxeEIhk24cSLOYiYRrPMxRZvA");
            var results = await uber.PriceEstimateAsync(37.6F, -122.3F, 37.8F, -122.5F); // San Francisco
         
            Assert.IsNotNull(results);
        }
    }
}
