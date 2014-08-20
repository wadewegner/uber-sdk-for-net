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

        public UberClient(string serverToken)
            : this(serverToken, "v1", new HttpClient())
        {
        }
        public UberClient(string serverToken, string apiVersion)
            : this(serverToken, apiVersion, new HttpClient())
        {
        }
        public UberClient(string serverToken, string apiVersion, HttpClient httpClient)
        {
            _serviceHttpClient = new ServiceHttpClient(TokenTypes.Server, apiVersion, serverToken, httpClient);
        }

        public Task<Products> ProductsAsync(float latitude, float longitude)
        {
            return _serviceHttpClient.HttpGetAsync<Products>(string.Format("products?latitude={0}&longitude={1}", latitude.ToString(), longitude.ToString()));
        }
    }
}
