using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.HttpClientFactory
{
    public class HttpConstructor
    {
        private IHttpClientFactory Client { get; }
        public HttpConstructor (IHttpClientFactory client)
        {
            Client = client;
        }
        public Task<HttpResponseMessage> CallAsync(string url)
        {
            var client = Client.CreateClient("Labirinth");
            return client.GetAsync(url);
        }
    }
}
