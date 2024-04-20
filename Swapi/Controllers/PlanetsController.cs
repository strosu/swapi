using Microsoft.AspNetCore.Mvc;
using Swapi.Models;
using Swapi.Services;

namespace Swapi.Controllers
{
    [ApiController]
    public class PlanetsController : ControllerBase
    {
        private readonly IMetadataFinder _metadataFinder;

        public PlanetsController(IMetadataFinder metadataFinder)
        {
            _metadataFinder = metadataFinder;
        }

        [HttpGet("planets/{planetId}")]
        public async Task<Planet> GetPlanet(int planetId)
        {
            return await _metadataFinder.GetSingleMetadata<Planet>(planetId);
        }

        [HttpGet("planets")]
        public async Task<IList<Planet>> GetPlanets()
        {
            return [];
        }
    }
}
