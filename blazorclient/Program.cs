using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using blazorclient.Services;

namespace blazorclient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            /*builder.HostEnvironment.BaseAddress*/
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:3500") });
            builder.Services.AddScoped<ICalculator, CalculatorService>();

            await builder.Build().RunAsync();
        }
    }
}
