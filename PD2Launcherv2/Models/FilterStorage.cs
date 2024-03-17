using Newtonsoft.Json;

namespace PD2Launcherv2.Models
{
    namespace ProjectDiablo2Launcherv2.Models
    {
        public class FilterStorage
        {
            public ETaggedData<List<FilterAuthor>> AuthorsList { get; set; }
            public ETaggedData<List<FilterFile>> AuthorsFilterList { get; set; }
            public FilterFile SelectedFilter { get; set; }
            public FilterFile SelectedAuthor { get; set; }
        }

        public class ETaggedData<T>
        {
            public string ETag { get; set; }
            public T Data { get; set; }
        }

        public class FilterAuthor
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("author")]
            public string Author { get; set; }
        }

        public class FilterFile
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("download_url")]
            public string DownloadUrl { get; set; }

            [JsonProperty("path")]
            public string Path { get; set; }

            [JsonProperty("sha")]
            public string Sha { get; set; }

            [JsonProperty("size")]
            public int Size { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty("git_url")]
            public string GitUrl { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}