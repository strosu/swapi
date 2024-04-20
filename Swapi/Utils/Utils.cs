namespace Swapi
{
    public static class Utils
    {
        public static int RoundUpDivision(int numerator, int denumerator) 
            => (numerator + denumerator - 1) / denumerator;

        public static List<string> SplitCommaSeparated(this string input)
            => input.Split(',').ToList();
    }

    public class UrlUtils
    {
        // Relies on the pattern of all resource URLs ending with their ID. Would obviously be more complex for other scenarios
        public static int ConvertToId(string url)
        {
            // Remove any trailing / and get the latest element (if it's an int)
            var idString = url.Trim('/').Split('/').Last();
            if (!int.TryParse(idString, out var id))
            {
                throw new ArgumentException($"Could not extract the id from url {url}");
            }

            return id;
        }
    }
}
