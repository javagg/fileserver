using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment environment;

        public HomeController(IHostingEnvironment environment) => this.environment = environment;

        public IActionResult Index() => new OkObjectResult("It works!");

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var uploads = Path.Combine(environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            if (file.Length > 0)
            {
                using (var stream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    await file.CopyToAsync(stream);
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
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
