using System.Text.Json.Serialization;

namespace Swapi.Models.Repository
{
    public class ResourceList<T>
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? NextPageUrl { get; set; }

        [JsonPropertyName("previous")]
        public string? PreviousPageUrl { get; set; }

        [JsonPropertyName("results")]
        public List<T> Results { get; set; }
    }
}
