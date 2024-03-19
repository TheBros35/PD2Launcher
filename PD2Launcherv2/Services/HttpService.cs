using Newtonsoft.Json;
using PD2Launcherv2.Models.ProjectDiablo2Launcherv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PD2Launcherv2.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        public HttpService()
        {
            _httpClient = new HttpClient();
            //add common headers as needed
        }

        public async Task<string> GetCallAsync(string clientFilesUrl)
        {
            var response = await _httpClient.GetAsync(clientFilesUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new HttpRequestException($"Error fetching game files: {response.StatusCode}");
        }

        public async Task<string> GetCallWithETagAsync(string filterUrl,string etag = null)
        {
      
            var request = new HttpRequestMessage(HttpMethod.Get, filterUrl);

            if (!string.IsNullOrWhiteSpace(etag))
            {
                request.Headers.IfNoneMatch.TryParseAdd(etag);
            }

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                // Handle the case where the content has not changed.
                return null; // or your preferred method of indicating no change
            }

            throw new HttpRequestException($"Error fetching filters: {response.StatusCode}");
        }

        public async Task<ETaggedData<List<FilterAuthor>>> GetFilterAuthorsAsync(string etag)
        {
            var requestUrl = "https://raw.githubusercontent.com/Project-Diablo-2/LootFilters/main/filters.json";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            if (!string.IsNullOrWhiteSpace(etag))
            {
                request.Headers.IfNoneMatch.TryParseAdd(etag);
            }

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var authors = JsonConvert.DeserializeObject<List<FilterAuthor>>(responseBody);
                var responseEtag = response.Headers.ETag?.Tag;

                return new ETaggedData<List<FilterAuthor>>
                {
                    ETag = responseEtag,
                    Data = authors
                };
            }
            else
            {
                // Handle different response status codes appropriately
                return null;
            }
        }
    }
}
