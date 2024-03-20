using Dropbox.Api.Files;
using Newtonsoft.Json;
using PD2Launcherv2.Interfaces;
using PD2Launcherv2.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PD2Launcherv2.Helpers
{
    public class FilterHelpers
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorage _localStorage;
        private const string FilterAuthorUrl = "https://raw.githubusercontent.com/Project-Diablo-2/LootFilters/main/filters.json";

        public FilterHelpers(HttpClient httpClient, ILocalStorage localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        private async Task<HttpResponseMessage> GetAsync(string url, string eTag = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(eTag))
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue($"\"{eTag}\""));
            }
            Debug.WriteLine($"eTag sent out {eTag}");
            return await _httpClient.SendAsync(request);
        }

        public async Task FetchAndStoreFilterAuthorsAsync()
        {
            Debug.WriteLine("\nstart FetchAndStoreFilterAuthorsAsync");
            try
            {
                var storedData = _localStorage.LoadSection<Pd2AuthorList>(StorageKey.Pd2AuthorList);
                var eTag = storedData?.StorageETag ?? string.Empty;

                var response = await GetAsync(FilterAuthorUrl, eTag);
                Debug.WriteLine($"ETAG: {eTag}");
                Debug.WriteLine($"response.StatusCode {response.StatusCode}");
                Debug.WriteLine($"response.Content {response.Content}");
                Debug.WriteLine($"response.ReasonPhrase {response.ReasonPhrase}");
                Debug.WriteLine($"response.Version {response.Version}");
                // Iterate over all headers
                foreach (var header in response.Headers)
                {
                    Debug.WriteLine($"Header: {header.Key}, Value: {string.Join(", ", header.Value)}");
                }


                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    // Data has not changed; no need to update
                    Console.WriteLine($"Code 304? {System.Net.HttpStatusCode.NotModified}");
                    Console.WriteLine("Filter authors data not changed.");
                    return;
                }

                Debug.WriteLine($"response.IsSuccessStatusCode {response.IsSuccessStatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var authors = JsonConvert.DeserializeObject<List<FilterAuthor>>(content);
                    Debug.WriteLine("is authors null?: " + authors != null);
                    Debug.WriteLine($"content {content}");
                    Debug.WriteLine($"authors {authors}");
                    Debug.WriteLine($"ETag Header: {response.Headers.ETag?.Tag}");
                    Debug.WriteLine($"2nd ETag Header: {response.Headers.ETag.ToString}");
                    var eTagValue = response.Headers.ETag?.Tag?.Trim('"');
                    Debug.WriteLine($"eTagValue {eTagValue}");
                    if (authors != null)
                    {
                        Pd2AuthorList eTaggedData = new()
                        {
                            StorageETag = eTagValue,
                            StorageAuthorList = authors
                        };

                        // Serialize eTaggedData to JSON for debugging
                        string eTaggedDataJson = JsonConvert.SerializeObject(eTaggedData, Formatting.Indented);
                        Debug.WriteLine($"eTaggedData JSON: {eTaggedDataJson}");

                        Debug.WriteLine($"eTaggedData: {eTaggedData}");
                        Debug.WriteLine($"final response.Headers.ETag?.Tag,: {response.Headers.ETag?.Tag}");

                        _localStorage.Update(StorageKey.Pd2AuthorList, eTaggedData);
                        Console.WriteLine("Filter authors data updated.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to fetch filter authors.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching filter authors: {ex.Message}");
            }
            Debug.WriteLine("end FetchAndStoreFilterAuthorsAsync\n");
        }
    }
}