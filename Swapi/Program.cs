
using Swapi.Services;
using Swapi.Services.Http;

namespace Swapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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
            builder.Services.AddScoped<IMetadataAggregator, MetadataAggregator>();
            builder.Services.AddScoped<IMetadataRetrieverFactory, MetadataRetrieverFactory>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
