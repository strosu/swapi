using Swapi.Models.Repository;

namespace Swapi.Models
{
    /// <summary>
    /// Gets the relevant API endpoint that corresponds to the current scenario (single item or a list of results)
    /// </summary>
    public static class MetadataConfiguration
    {
        private static Dictionary<Type, string> EntityMapping = new()
        {
            { typeof(SwapiPlanet), "https://swapi.dev/api/planets/" }
        };

        public static string GetEntityUrl<T>(int id)
        {
            ValidateEntityType<T>();

            return $"{EntityMapping[typeof(T)]}{id}";
        }

        public static string GetEntityPage<T>(int page)
        {
            ValidateEntityType<T>();
            return $"{EntityMapping[typeof(T)]}?page={page}";
        }

        private static void ValidateEntityType<T>()
        {
            if (!EntityMapping.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"Type {typeof(T)} is not configured");
            }
        }
    }
}
