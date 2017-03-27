using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace WebApplication1
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment environment;
        private IEnumerable<string> aa;
        public HomeController(IHostingEnvironment environment) => this.environment = environment;

       public IActionResult Index() => new OkObjectResult("It works!");

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var uploads = Path.Combine(environment.ContentRootPath, "wwwroot");
            if (file.Length > 0)
            {
                using (var stream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    await file.CopyToAsync(stream);
                Console.WriteLine($"[{DateTime.Now}] {file.FileName} saved!");
            }
            return new OkObjectResult("uploaded!");
        }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath);
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDirectoryBrowser();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot")),
                EnableDirectoryBrowsing = true
            });
            app.UseMvcWithDefaultRoute();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
