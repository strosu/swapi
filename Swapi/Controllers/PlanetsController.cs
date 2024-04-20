using Microsoft.AspNetCore.Mvc;
using Swapi.Models.DTO;
using Swapi.Models.Repository;
using Swapi.Services;

namespace Swapi.Controllers
{
    [ApiController]
    public class PlanetsController : ControllerBase
    {
        private readonly IMetadataAggregator _metadataFinder;

        public PlanetsController(IMetadataAggregator metadataFinder)
        {
            _metadataFinder = metadataFinder;
        }

        [HttpGet("planets/{planetId}")]
        public async Task<Planet> GetPlanet(int planetId)
        {
            return await _metadataFinder.GetSingleMetadataAsync<SwapiPlanet>(planetId);
        }

        [HttpGet("planets")]
        public async Task<IEnumerable<Planet>> GetPlanets()
        {
            var swapiPlanets = await _metadataFinder.GetMetadataSetAsync<SwapiPlanet>();
            return swapiPlanets.Select(x => (Planet)x);
        }
    }
}
