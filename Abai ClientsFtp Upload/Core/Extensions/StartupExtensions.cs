using Core.Interfaces;
using Core.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.Processors.Security;
//using NSwag.Generation.Processors.Security;
namespace Core.Extensions
{
    public static class StartupExtensions
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        #region Database

        /// <summary>
        /// Add SQL Server Context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSqlServerContext<T>(this IServiceCollection services, AppConfiguration configuration) where T : DbContext
        {
            try
            {
                services.AddDbContext<T>((serviceProvider, options) =>
                {
                    options.UseSqlServer(
                        configuration.Database.ConnectionString,
                        options =>
                        {
                            options.UseQuerySplittingBehavior(configuration.Database.QuerySplittingBehavior == 1 ? QuerySplittingBehavior.SplitQuery : QuerySplittingBehavior.SingleQuery);
                            options.CommandTimeout(configuration.Database.Timeout);
                            options.EnableRetryOnFailure(); // Reintentar si falla
                        });

                    if (configuration.Database.SensitiveDataLogging)
                        options.EnableSensitiveDataLogging();

                });
            }
            catch (Exception ex)
            {
                Log.Error("AddSqlServerContext error");
                Log.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// Initialize Database with migrations and Seed if requiered.
        /// </summary>
        /// <remarks>Custom StartupExtension</remarks>
        /// <param name="app"></param>
        public static void UseDatabase<T>(this IApplicationBuilder app) where T : DbContext, IContext
        {
            try
            {
                using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                using var context = serviceScope.ServiceProvider.GetService<T>();

                context?.Database.Migrate();
                context?.Seed();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Swagger / OpenAPI

        public static void AddOpenApi(this IServiceCollection services, AppConfiguration configuration)
        {
            try
            {
                services.AddOpenApiDocument(config =>
                {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = configuration.API.Version;
                        document.Info.Title = configuration.API.Title;
                        document.Info.Description = configuration.API.Description;
                        document.Info.TermsOfService = configuration.API.Description;
                        document.Info.Contact = new NSwag.OpenApiContact
                        {
                            Name = configuration.API.Contact.Name,
                            Email = configuration.API.Contact.Email,
                            Url = configuration.API.Contact.Url
                        };
                        document.Info.License = new NSwag.OpenApiLicense
                        {
                            Name = configuration.API.License.Name,
                            Url = configuration.API.License.Url
                        };
                    };

                    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("apiKey"));
                    config.DocumentProcessors.Add(new SecurityDefinitionAppender("apiKey", new NSwag.OpenApiSecurityScheme()
                    {
                        Type = configuration.API.Authentication.Type == (int)NSwag.OpenApiSecuritySchemeType.ApiKey ? NSwag.OpenApiSecuritySchemeType.ApiKey : NSwag.OpenApiSecuritySchemeType.Basic,
                        Name = "Authorization",
                        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                        Description = configuration.API.Authentication.Description,
                    }));

                });
            }
            catch (Exception ex)
            {
                Log.Error("AddOpenApi error");
                Log.Error(ex);
            }

        }

        #endregion
    }
}
