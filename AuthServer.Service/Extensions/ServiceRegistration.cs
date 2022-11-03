using AuthServer.Core.Services;
using AuthServer.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuthServer.Service.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceRegistration(this IServiceCollection services)
        {
            var assmebly = Assembly.GetExecutingAssembly();
            services.AddScoped<ITokenService,TokenService>();
            services.AddAutoMapper(assmebly);
            return services;
        }
    }
}
