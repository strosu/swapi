namespace Swapi.Middleware.RateLimiter
{
    public interface IPartitionStrategy
    {
        string GetPartition();
    }

    public class IPPartitionStrategy(IHttpContextAccessor httpContextAccessor) : IPartitionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetPartition()
        {
            var forwardedIp = _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].ToString();

            if (string.IsNullOrEmpty(forwardedIp))
            {
                var originalIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(originalIp))
                {
                    // If we couldn't get any relevant information to this point, just put the user in the default group.
                    // This will share the same rate limits with the rest of the unauthenticated users for which we failed to get the IPs
                    return DefaultPartition;
                }

                return originalIp;
            }

            return forwardedIp;
        }

        private static readonly string DefaultPartition = "CommonPartition";
    }
}
