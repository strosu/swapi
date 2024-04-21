using Microsoft.Extensions.Options;
using RedisRateLimiting.AspNetCore;
using StackExchange.Redis;
using Swapi.Middleware.RateLimiter;
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
                // Set a policy that applies to the entire set of users
                // That is, in span of 30 seconds, we don't accept more than 20 connections in total
                options.AddRedisSlidingWindowLimiter("aggregateRequest", opt =>
                {
                    opt.ConnectionMultiplexerFactory = () => multiplexer;
                    opt.PermitLimit = 20;
                    opt.Window = TimeSpan.FromSeconds(30);
                });

                options.AddPolicy<string, RateLimiterPolicy>("singleRequest");
            });

            // TODO - remove this
            // Temporarily here to unblock myself. This should never be used in a production system
            DisableSSL(builder);

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<SwapiExceptionFilter>();
            })
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Logging.AddConsole();
            RegisterServices(builder, multiplexer);

            var app = builder.Build();

            app.UseRateLimiter();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }

        /// <summary>
        /// Needing this since the SSL cert for Swapi expired
        /// </summary>
        /// <param name="builder"></param>
        private static void DisableSSL(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient(Options.DefaultName, c =>
            {
                // ...
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, certChain, policyErrors) => true
                };
            });
        }

        private static void RegisterServices(WebApplicationBuilder builder, ConnectionMultiplexer multiplexer)
        {
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IRetryService, ExponentialBackoffRetryService>();
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddScoped<IMetadataRetriever, MetadataRetriever>();
            builder.Services.AddScoped<IMetadataRetrieverFactory, MetadataRetrieverFactory>();
            builder.Services.AddSingleton<IConnectionMultiplexer>(x => multiplexer);

            builder.Services.AddSingleton<IPartitionStrategy, IPPartitionStrategy>();

            RegisterAggregator(builder);
        }

        private static void RegisterAggregator(WebApplicationBuilder builder)
        {
            if (builder.Configuration.GetValue<bool>(Constants.CachingEnabledConfig))
            {
                // If caching is enabled, we're registering a CachedMetadataAggregator as the implementation for IMetadataAggregator,
                // which just wraps the MetadataAggregator
                // It will use the MetadataAggregator to send out the actual requests when it can't find them in the cache
                builder.Services.AddScoped<MetadataAggregator>();
                builder.Services.AddScoped<IMetadataAggregator>(provider =>
                {
                    var networkAggregator = provider.GetService<MetadataAggregator>();
                    var logger = provider.GetService<ILogger<CachedMetadataAggregator>>();
                    var multiplexer = provider.GetService<IConnectionMultiplexer>();
                    return new CachedMetadataAggregator(networkAggregator, multiplexer, logger);
                });
            }
            else
            {
                // Otherwise, just register a regular MetadataAggregator as the default implementation for IMetadataAggregator
                builder.Services.AddScoped<IMetadataAggregator, MetadataAggregator>();
            }

        }
    }
}
