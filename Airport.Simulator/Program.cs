//#define TEST
using Airport.Models.Interfaces;
using Airport.Services;
using Airport.Services.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Airport.Simulator
{
    public class Program
    {
        private static ILogger<Program> _logger = null!;

        public static async Task Main(params string[] args)
        {
            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Environment.CurrentDirectory).AddJsonFile(
                        "appsettings.json",
                        optional: false,
                        reloadOnChange: true);
                })
                .ConfigureServices(services =>
                {
                    // Http client
                    services.AddHttpClient<IFlightLauncherService, FlightLauncherService>()
                    .AddPolicyHandler(GetRetryPolicy());
                    services.AddScoped<IFlightGenerator, FlightGenerator>();
                })
                .Build();

            _logger = host.Services.GetRequiredService<ILogger<Program>>();
            IFlightLauncherService flightLauncherService = host.Services.GetRequiredService<IFlightLauncherService>();
            var startResponse = await flightLauncherService.StartAsync();
#if TEST
            await Console.Out.WriteLineAsync(startResponse.StatusCode.ToString());
            await flightLauncherService.LaunchManyAsync(args); 
#else
            await flightLauncherService.SetFlightTimeoutAsync(TimeSpan.FromMilliseconds(1101)/*, Models.Enums.FlightType.Departure*/);
#endif
            await host.RunAsync();
        }

        // Adds Polly's policy for Http Retries with exponential backoff
        private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // Exception handler
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            _logger.LogError(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}