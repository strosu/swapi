namespace Swapi.Models
{
    public static class MetadataConfiguration
    {
        private static Dictionary<Type, string> EntityMapping = new()
        {
            { typeof(Planet), "https://swapi.dev/api/planets/" }
        };

        public static string GetEntityUrl<T>(int id)
        {
            ValidateEntityType<T>();

            return $"{EntityMapping[typeof(T)]}{id}";
        }

        public static string GetAllUrl<T>()
        {
            ValidateEntityType<T>();

            return EntityMapping[typeof(T)];
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
