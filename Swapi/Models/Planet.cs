using System.Text.Json.Serialization;

namespace Swapi.Models
{
    public class Planet
    {
        /// <summary>
        /// the hypermedia URL of this resource
        /// </summary>
        [JsonPropertyName("url")]
        public string IdentifyingUrl { get; set; }

        /// <summary>
        /// The name of this planet
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The diameter of this planet in kilometers.
        /// </summary>
        [JsonPropertyName("diameter")]
        public string Diameter { get; set; }

        /// <summary>
        /// The number of standard hours it takes for this planet to complete a single rotation on its axis.
        /// </summary>
        [JsonPropertyName("rotation_period")]
        public string RotationPeriod { get; set; }

        /// <summary>
        ///  The number of standard days it takes for this planet to complete a single orbit of its local star.
        /// </summary>
        [JsonPropertyName("orbital_period")]
        public string OrbitalPeriod { get; set; }

        /// <summary>
        /// A number denoting the gravity of this planet, where "1" is normal or 1 standard G. "2" is twice or 2 standard Gs. "0.5" is half or 0.5 standard Gs.
        /// </summary>
        [JsonPropertyName("gravity")]
        public string GravityMultiplier {  get; set; }

        /// <summary>
        /// The average population of sentient beings inhabiting this planet.
        /// </summary>
        [JsonPropertyName("population")]
        public string PopulationCountAverage { get; set; }

        /// <summary>
        /// The climate of this planet.Comma separated if diverse.
        /// </summary>
        [JsonPropertyName("climate")]
        public string Climates { get; set; }

        /// <summary>
        /// The terrain of this planet.Comma separated if diverse.
        /// </summary>
        [JsonPropertyName("terrain")]
        public string Terrains { get; set; }

        /// <summary>
        /// The percentage of the planet surface that is naturally occurring water or bodies of water.
        /// </summary>
        [JsonPropertyName("surface_water")]
        public string WaterPercentage { get; set; }

        /// <summary>
        /// An array of People URL Resources that live on this planet.
        /// </summary>
        [JsonPropertyName("residents")]
        public List<string> ResidentUrls { get; set; }

        /// <summary>
        /// An array of Film URL Resources that this planet has appeared in
        /// </summary>
        [JsonPropertyName("films")]
        public List<string> RelatedFilms { get; set; }

        /// <summary>
        /// The ISO 8601 date format of the time that this resource was created
        /// </summary>
        [JsonPropertyName("created")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The ISO 8601 date format of the time that this resource was edited
        /// </summary>
        [JsonPropertyName("edited")]
        public DateTime LastEditedTime { get; set; }
    }
}
