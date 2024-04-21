namespace Swapi.Services.HttpRequest
{
    public class ProxyRateLimitedException(string message): Exception(message)
    {
    }

    public class NotFoundException(string message): Exception(message) { }
}
