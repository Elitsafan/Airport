//#define SQLITE
using Airport.Data;
using Airport.Data.Repositories;
using Airport.Models;
using Airport.Models.DTOs;
using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Airport.Services;
using Airport.Services.Factories;
using Airport.Services.Mappers;
using Airport.Services.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Airport.API
{
    public class Program
    {
#if !SQLITE
        private static string SqlServerConnectionString = "SqlServerConnection";
#else
        private static string sqliteConnectionString = "SqliteConnection";
#endif
        private static string defaultClientOrigin = "defaultClientOrigin";

        public static void Main(string[] args)
        {
            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            var builder = WebApplication.CreateBuilder(args);
            Configuration = builder.Configuration;
#if !SQLITE
            builder.Services.AddDbContext<AirportContext>(options => options
                .ConfigureWarnings(builder => builder.Ignore(RelationalEventId.MultipleCollectionIncludeWarning))
                .UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetConnectionString(SqlServerConnectionString)), ServiceLifetime.Transient);
#else
            builder.Services.AddDbContext<AirportContext>(options => options
                .ConfigureWarnings(builder => builder.Ignore(RelationalEventId.MultipleCollectionIncludeWarning))
                .UseLazyLoadingProxies()
                .UseSqlite(Configuration.GetConnectionString(sqliteConnectionString)));
#endif
            builder.Services.AddSignalR(/*options => options.EnableDetailedErrors = true*/);
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(Configuration.GetValue<string>(defaultClientOrigin)!)
                            .AllowAnyHeader()
                            .WithMethods("GET")
                            .AllowCredentials();
                    });
            });
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddSingleton<IRouteLogicFactory, RouteLogicFactory>();
            builder.Services.AddSingleton<IFlightLogicFactory, FlightLogicFactory>();
            builder.Services.AddSingleton<IFlightCreatorFactory, FlightCreatorFactory>();
            builder.Services.AddSingleton<IStationLogicFactory, StationLogicFactory>(
                serviceProvider => new StationLogicFactory(serviceProvider));
            builder.Services.AddSingleton<IDirectionLogicFactory, DirectionLogicFactory>();
            builder.Services.AddSingleton<ITrafficLightLogicFactory, TrafficLightLogicFactory>();
            builder.Services.AddSingleton<IRouteLogicProvider, RouteLogicProvider>();
            builder.Services.AddSingleton<IStationLogicProvider, StationLogicProvider>();
            builder.Services.AddSingleton<IDirectionLogicProvider, DirectionLogicProvider>();
            builder.Services.AddSingleton<ITrafficLightLogicProvider, TrafficLightLogicProvider>();
            builder.Services.AddSingleton<IAirportHubHandlerRegistrar, AirportHubHandlerRegistrar>();
            builder.Services.AddScoped<IAirportService, AirportService>();
            builder.Services.AddScoped<IRouteRepository, RouteRepository>();
            builder.Services.AddScoped<IStationRepository, StationRepository>();
            builder.Services.AddScoped<IStationFlightRepository, StationFlightRepository>();
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<ITrafficLightRepository, TrafficLightRepository>();
            builder.Services.AddScoped<IEntityMapper<Flight, IFlight>, FlightMapper>();
            builder.Services.AddScoped<IEntityMapper<Station, StationDTO>, StationMapper>();
            builder.Services.AddScoped<IEntityMapper<IFlightCreator, IFlightDTOFactory>, FlightCreatorAdapter>();
            builder.Services.AddControllers()
                .AddNewtonsoftJson();
            var app = builder.Build();

            using (var scoped = app.Services.CreateScope())
            {
                var ctx = scoped.ServiceProvider.GetRequiredService<AirportContext>();
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
            app.UseCors();
            app.MapControllers();
            app.MapHub<AirportHub>("/airporthub");
            app.Run();
        }

        private static IConfiguration Configuration { get; set; } = null!;
        // Exception handler
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}