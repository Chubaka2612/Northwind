using Microsoft.AspNetCore.Hosting;
using Northwind.Web;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
             .ConfigureLogging((context, logging) =>
             {
                 var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "app-log.txt"); 
                 logging.AddFile(logFilePath, LogLevel.Information);
             })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}