using Newtonsoft.Json;

namespace PD2Launcherv2.Models
{
    public class FilterAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "null";

        [JsonProperty("url")]
        public string Url { get; set; } = "null";

        [JsonProperty("author")]
        public string Author { get; set; } = "null";
    }
}