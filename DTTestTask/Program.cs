using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DTTestTask.Services;
using Microsoft.Extensions.Configuration;
using DTTestTask.Models;
using Microsoft.EntityFrameworkCore;
static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient();
                services.AddLogging(configure => configure.AddConsole());
                services.AddTransient<CsvService>();
                services.AddTransient<TripRepositoryService>();
                services.AddDbContext<TripContext>((serviceProvider, options) =>
                {
                    var config = serviceProvider.GetRequiredService<IConfiguration>();
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection")!);
                    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                });
                services.AddDbContextFactory<TripContext>();
            });

using IHost host = CreateHostBuilder(args).Build();

var contextFactory = host.Services.GetRequiredService<IDbContextFactory<TripContext>>();
using var context = contextFactory.CreateDbContext();
await context.Database.EnsureCreatedAsync();

var csvUrl = host.Services.GetRequiredService<IConfiguration>().GetValue<string>("CsvConfig:CsvFileUrl");
var stream = await host.Services.GetRequiredService<CsvService>().ReadCsvUrlAsync(csvUrl!);

var tripRepositoryService = host.Services.GetRequiredService<TripRepositoryService>();
await tripRepositoryService.InsertCsvDataToDatabaseAsync(context, stream);