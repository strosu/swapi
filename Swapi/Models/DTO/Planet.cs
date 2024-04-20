using Swapi.Models.Repository;
using System;

namespace Swapi.Models.DTO
{
    /// <summary>
    /// Our customer facing API model for a planet
    /// </summary>
    public class Planet
    {
        public int Id { get; set; }

        /// <summary>
        /// The name of this planet
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The diameter of this planet in kilometers.
        /// </summary>
        public string Diameter { get; set; }

        /// <summary>
        /// The number of standard hours it takes for this planet to complete a single rotation on its axis.
        /// </summary>
        public string RotationPeriod { get; set; }

        /// <summary>
        ///  The number of standard days it takes for this planet to complete a single orbit of its local star.
        /// </summary>
        public string OrbitalPeriod { get; set; }

        /// <summary>
        /// A number denoting the gravity of this planet, where "1" is normal or 1 standard G. "2" is twice or 2 standard Gs. "0.5" is half or 0.5 standard Gs.
        /// </summary>
        public string GravityMultiplier { get; set; }

        /// <summary>
        /// The average population of sentient beings inhabiting this planet.
        /// </summary>
        public string PopulationCountAverage { get; set; }

        /// <summary>
        /// List of the climates of this planet
        /// </summary>
        public List<string> Climates { get; set; }

        /// <summary>
        /// List of the types of terrain for this planet
        /// </summary>
        public List<string> Terrains { get; set; }

        /// <summary>
        /// The percentage of the planet surface that is naturally occurring water or bodies of water.
        /// </summary>
        public string WaterPercentage { get; set; }

        /// <summary>
        /// An array of IDs of the people living on this planet
        /// </summary>
        public IEnumerable<int> ResidentIds { get; set; }

        /// <summary>
        /// An array of Film URLs in which this planet was featured
        /// </summary>
        public IEnumerable<int> RelatedFilmIds { get; set; }

        /// <summary>
        /// The ISO 8601 date format of the time that this resource was created
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The ISO 8601 date format of the time that this resource was edited
        /// </summary>
        public DateTime LastEditedTime { get; set; }

        public static implicit operator Planet(SwapiPlanet swapiPlanet)
        {
            return new Planet
            {
                Id = UrlUtils.ConvertToId(swapiPlanet.IdentifyingUrl),
                Name = swapiPlanet.Name,
                Diameter = swapiPlanet.Diameter,
                RotationPeriod = swapiPlanet.RotationPeriod,
                OrbitalPeriod = swapiPlanet.OrbitalPeriod,
                GravityMultiplier = swapiPlanet.GravityMultiplier,
                PopulationCountAverage = swapiPlanet.PopulationCountAverage,
                Climates = swapiPlanet.Climates.SplitCommaSeparated(),
                Terrains = swapiPlanet.Terrains.SplitCommaSeparated(),
                ResidentIds = swapiPlanet.ResidentUrls.Select(UrlUtils.ConvertToId),
                RelatedFilmIds = swapiPlanet.RelatedFilms.Select(UrlUtils.ConvertToId),
                CreationTime = swapiPlanet.CreationTime,
                LastEditedTime = swapiPlanet.LastEditedTime
            };
        }
    }
}
