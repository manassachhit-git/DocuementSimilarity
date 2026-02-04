using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Service
{
    public class SelfCorrectorLLamaService
    {
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly HttpClient _client;

        public SelfCorrectorLLamaService(string apiKey, string endpoint)
        {
            _apiKey = apiKey;
            _endpoint = endpoint;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> SendAsync(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            string JsonContent =
                jsonDocument.RootElement
                    .GetProperty("choices")[0]!
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "{}";
            return JsonContent;
        }
    }
}
