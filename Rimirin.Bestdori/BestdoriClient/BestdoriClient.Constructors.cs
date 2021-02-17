using Rimirin.Bestdori.JsonConverters;
using System.Net.Http;
using System.Text.Json;

namespace Rimirin.Bestdori
{
    public partial class BestdoriClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly bool isHttpClientDisposable;

        public BestdoriClient(HttpClient httpClient = null, bool isHttpClientDisposable = false)
        {
            this.httpClient = httpClient ?? new HttpClient();
            this.isHttpClientDisposable = httpClient == null || isHttpClientDisposable;
            this.jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            jsonOptions.Converters.Add(new UnixTimeStringJsonConverter());
        }
    }
}