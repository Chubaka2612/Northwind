
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Abstractions;
using Northwind.Bll.Services;
using Northwind.Dal.Abstractions;
using Northwind.Dal;
using Northwind.Logger;
using Northwind.Utils.Caching;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;


public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var connection = Configuration.GetConnectionString("NorthwindDb");

        services.AddDbContext<NorthwindDbContext>(options =>
            options.UseSqlServer(connection));

        services.AddScoped<IUnitOfWork, NorthwindUnitOfWork>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierService, SupplierService>();

        services.Configure<ImageCachingOptions>(Configuration.GetSection("ImageCachingOptions"));
        services.AddMemoryCache();

        // Add Logging Action Filter if enabled
        if (Configuration.GetSection("LogActionFilterOn").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
            services.AddScoped(sp => new LoggingActionFilter(sp.GetRequiredService<ILogger<LoggingActionFilter>>(), logParameters: true));
        }

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        // Register NSwag for OpenAPI/Swagger documentation /swagger/v1/swagger.json
        services.AddOpenApiDocument(config =>
        {
            config.Title = "Northwind API";
            config.Version = "v1";
            config.Description = "API documentation for the Northwind project.";
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        // Log application startup
        logger.LogInformation("Application starting up. Location: {Location}", Directory.GetCurrentDirectory());

        // Log configuration values
        var configValues = Configuration.AsEnumerable()
            .Where(c => !string.IsNullOrWhiteSpace(c.Value))
            .Select(c => $"{c.Key}: {c.Value}")
            .ToList();
        logger.LogInformation("Configuration values: {ConfigValues}", string.Join(", ", configValues));

        app.UseRouting();
        app.UseAuthorization();

        app.UseMiddleware<ImageCachingMiddleware>(
            app.ApplicationServices.GetRequiredService<ILogger<ImageCachingMiddleware>>(),
            app.ApplicationServices.GetRequiredService<IMemoryCache>(),
            app.ApplicationServices.GetRequiredService<IOptions<ImageCachingOptions>>().Value);

        DefaultFilesOptions options = new DefaultFilesOptions();
        options.DefaultFileNames.Clear();
        options.DefaultFileNames.Add("index.html");
        app.UseDefaultFiles(options);
        app.UseStaticFiles();

        app.UseOpenApi(); 
        app.UseSwaggerUi(); 

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

