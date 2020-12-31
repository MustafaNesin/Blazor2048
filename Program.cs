using System.Net.Http;
using System.Threading.Tasks;
using Blazor2048.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor2048
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddScoped(_ => new HttpClient
                {
                    BaseAddress = new(builder.HostEnvironment.BaseAddress)
                })
                .AddScoped<GameService>();

            await builder.Build().RunAsync();
        }
    }
}