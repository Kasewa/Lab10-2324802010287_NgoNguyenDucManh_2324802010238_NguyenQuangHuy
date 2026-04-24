using ASC.Business;
using ASC.Business.Interfaces;
using ASC.Web.Data;
using ASC.Web.Services;
using Humanizer.Configuration;
using Microsoft.Extensions.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ASC.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMyDependencyGroup(this IServiceCollection services, IConfiguration configuration)
        {
            // Add memory cache
            services.AddMemoryCache();

            // Add session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add navigation cache operations
            services.AddSingleton<INavigationCacheOperations, NavigationCacheOperations>();

            services.AddAuthentication().AddGoogle(options =>
            {
                //IConfigurationSection googleAuthNSection = config.GetSection("Authentication:Google");
                IConfigurationSection googleAuthNSection = configuration.GetSection("Google:Identity");
                options.ClientId = googleAuthNSection["ClientId"]!;
                options.ClientSecret = googleAuthNSection["ClientSecret"]!;

                options.CorrelationCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            });
            //Add MasterDataOperations
            services.AddScoped<IMasterDataOperations, MasterDataOperations>();
            services.AddAutoMapper(typeof(ApplicationDbContext));
            //

            return services;
        }
    }
}
