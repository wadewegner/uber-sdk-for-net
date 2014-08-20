using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Uber.Models;

namespace Uber
{
    public class UberClient
    {
        private ServiceHttpClient _serviceHttpClient;
        public TokenTypes TokenType = TokenTypes.Server;

        public UberClient(string token)
            : this(token, "v1", new HttpClient())
        {
        }
        public UberClient(string token, string apiVersion)
            : this(token, apiVersion, new HttpClient())
        {
        }
        public UberClient(string token, string apiVersion, HttpClient httpClient)
        {
            _serviceHttpClient = new ServiceHttpClient(apiVersion, token, httpClient);
        }

        public Task<Products> ProductsAsync(float latitude, float longitude)
        {
            return _serviceHttpClient.HttpGetAsync<Products>(string.Format("products?latitude={0}&longitude={1}", latitude.ToString("R"), longitude.ToString("R")));
        }

        public Task<Products> PriceEstimateAsync(float startLatitude, float startLongitude, float endLatitude, float endLongitude)
        {
            return _serviceHttpClient.HttpGetAsync<Products>(string.Format("estimates/price?start_latitude={0}&start_longitude={1}&end_latitude={2}&end_longitude={3}", startLatitude.ToString("R"), startLongitude.ToString("R"), endLatitude.ToString("R"), endLongitude.ToString("R")));
        }
    }
}
