using RedisRateLimiting.AspNetCore;
using StackExchange.Redis;
using Swapi.Middleware;
using Swapi.Services;
using Swapi.Services.Caching;
using Swapi.Services.Http;
using Swapi.Services.HttpAggregator;

namespace Swapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var redisConnection = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis"));
            var multiplexer = ConnectionMultiplexer.Connect(redisConnection);

            builder.Services.AddRateLimiter(options =>
            {
                options.AddRedisSlidingWindowLimiter("aggregateRequest", opt =>
                {
                    opt.ConnectionMultiplexerFactory = () => multiplexer;
                    opt.PermitLimit = 1;
                    opt.Window = TimeSpan.FromSeconds(30);
                });

                options.AddPolicy<string, ClientIdRateLimiterPolicy>("singleRequest");
            });


            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Logging.AddConsole();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IRetryService, ExponentialBackoffRetryService>();
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddScoped<IMetadataRetriever, MetadataRetriever>();
            builder.Services.AddScoped<IMetadataRetrieverFactory, MetadataRetrieverFactory>();
            builder.Services.AddSingleton<IConnectionMultiplexer>(x => multiplexer);

            builder.Services.AddScoped<MetadataAggregator>();
            builder.Services.AddScoped<IMetadataAggregator>(provider =>
            {
                var networkAggregator = provider.GetService<MetadataAggregator>();
                var logger = provider.GetService<ILogger<CachedMetadataAggregator>>();
                var multiplexer = provider.GetService<IConnectionMultiplexer>();
                return new CachedMetadataAggregator(networkAggregator, multiplexer, logger);
            });


            var app = builder.Build();

            app.UseRateLimiter();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
