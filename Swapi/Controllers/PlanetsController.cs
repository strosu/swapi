using Microsoft.AspNetCore.Mvc;
using Swapi.Models;
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
            return await _metadataFinder.GetSingleMetadataAsync<Planet>(planetId);
        }

        [HttpGet("planets")]
        public async Task<IEnumerable<Planet>> GetPlanets()
        {
            return await _metadataFinder.GetMetadataSetAsync<Planet>();
        }
    }
}
