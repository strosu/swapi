namespace Swapi.Models
{
    public static class MetadataConfiguration
    {
        public static Dictionary<Type, string> EntityMapping = new()
        {
            { typeof(Planet), "https://swapi.dev/api/planets/" }
        };
    }
}
