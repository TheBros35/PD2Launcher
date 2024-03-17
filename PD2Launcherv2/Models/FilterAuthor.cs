using Newtonsoft.Json;

namespace ProjectDiablo2Launcherv2.Models
{
    public class FilterAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }
    }
}
