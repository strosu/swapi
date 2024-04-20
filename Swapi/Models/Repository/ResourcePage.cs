using System.Text.Json.Serialization;

namespace Swapi.Models.Repository
{
    /// <summary>
    /// Represents a page of resources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResourcePage<T>
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("results")]
        public List<T> Results { get; set; }
    }
}
