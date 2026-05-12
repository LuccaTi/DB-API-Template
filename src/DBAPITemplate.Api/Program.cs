using Mapster;
using MapsterMapper;
using DBAPITemplate.Application.Interfaces.Repositories;
using DBAPITemplate.Application.Interfaces.Services;
using DBAPITemplate.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentValidation;
using DBAPITemplate.Application.Mappers.Product;
using DBAPITemplate.Application.Services.Product;
using DBAPITemplate.Application.Validations.Product;

namespace APITemplate.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build())
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs/system_log_.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                shared: true);

            Log.Logger = loggerConfiguration.CreateBootstrapLogger();
            Log.Information("DB API Template, Starting up");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                Log.Information("Configuring builder");
                builder.Host.UseSerilog();

                #region DI Container

                builder.Services.AddDbContext<DBAPITemplate.Infrastructure.Data.AppDbContext>(options =>
                    options.UseInMemoryDatabase("DB_Template_Local"));
                Log.Information("Created in memory (RAM) database 'DB_Template_Local'");

                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                Log.Information("Repositories scope added");

                builder.Services.AddValidatorsFromAssemblyContaining<GetProductByIdRequestValidator>();
                Log.Information("Fluent validation added");

                builder.Services.AddScoped<IProductService, ProductService>();
                Log.Information("Services scope added");

                var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
                typeAdapterConfig.Scan(typeof(ProductMapperRegister).Assembly);
                builder.Services.AddSingleton(typeAdapterConfig);
                builder.Services.AddScoped<IMapper, ServiceMapper>();
                Log.Information("Mapster added");

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                Log.Information("Dependencies injected");

                #endregion

                var app = builder.Build();
                Log.Information("Builder configured and web application created");

                #region Middleware

                app.UseMiddleware<APITemplate.Host.Middleware.ExceptionHandlerMiddleware>();
                Log.Debug("ExceptionHandlerMiddleware added");

                app.UseStaticFiles();
                app.UseSwagger();
                app.UseSwaggerUI();

                // Automatically accesses swagger when clicking on the listened link (Now listening) - Console
                app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                #endregion

                Log.Information("All parameters have been loaded, starting the application...");

                var useSwaggerProduction = app.Configuration.GetValue<bool>("AppConfig:UseSwaggerProduction");
                if (!app.Environment.IsDevelopment() && useSwaggerProduction)
                {
                    // Always switch to use https
                    app.Lifetime.ApplicationStarted.Register(() =>
                    {
                        var address = app.Urls.FirstOrDefault();
                        if (address != null)
                        {
                            if (address.StartsWith("http://"))
                            {
                                address = address.Replace("http://", "https://");
                            }

                            address = address.Replace("0.0.0.0", "localhost");
                        }

                        var swaggerUrl = $"{address}/swagger";
                        Log.Debug("Program.cs", "Main", $" ===== Now listening on: {swaggerUrl} ===== ");
                        OpenBrowser(swaggerUrl);
                    });
                }

                Log.Information("Running application...");
                app.Run();

                Log.Information("Request to terminate received, stopping the application...");
                Log.Information("Application terminated.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void OpenBrowser(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Program.cs", "OpenBrowser", $"Error opening browser: {ex.Message}");
                throw;
            }
        }

    }
}
