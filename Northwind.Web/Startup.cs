
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Abstractions;
using Northwind.Bll.Services;
using Northwind.Dal.Abstractions;
using Northwind.Dal;
using Northwind.Logger;
using Northwind.Utils.Caching;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;


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

        if (Configuration.GetSection("LogActionFilterOn").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
            services.AddScoped(sp => new LoggingActionFilter(sp.GetRequiredService<ILogger<LoggingActionFilter>>(), logParameters: true));

            services.AddControllersWithViews(options =>
            {
                options.Filters.AddService<LoggingActionFilter>();
            });
        }
        services.AddHttpContextAccessor();
        services.AddMvc();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        logger.LogInformation("Application starting up. Location: {Location}", Directory.GetCurrentDirectory());

        var imageCachingOptions = app.ApplicationServices.GetRequiredService<IOptions<ImageCachingOptions>>().Value;

        var configValues = Configuration.AsEnumerable()
            .Where(c => !string.IsNullOrWhiteSpace(c.Value))
            .Select(c => $"{c.Key}: {c.Value}")
            .ToList();
        logger.LogInformation("Configuration values: {ConfigValues}", string.Join(", ", configValues));

        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.UseMiddleware<ImageCachingMiddleware>(
            app.ApplicationServices.GetRequiredService<ILogger<ImageCachingMiddleware>>(),
            app.ApplicationServices.GetRequiredService<IMemoryCache>(),
            imageCachingOptions);

        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
