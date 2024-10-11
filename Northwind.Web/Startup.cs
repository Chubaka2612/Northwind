
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Abstractions;
using Northwind.Bll.Services;
using Northwind.Dal;
using Northwind.Dal.Abstractions;


namespace Northwind.Web
{
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

            services.AddControllersWithViews();

            services.AddHttpContextAccessor();
        
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Application starting up. Location: {Location}", Directory.GetCurrentDirectory());

            var configValues = Configuration.AsEnumerable()
                .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                .Select(c => $"{c.Key}: {c.Value}")
                .ToList();
            logger.LogInformation("Configuration values: {ConfigValues}", string.Join(", ", configValues));

            app.UseMiddleware<ErrorHandlerMiddleware>();

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
}
