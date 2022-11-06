using AuthServer.Core.Services;
using AuthServer.Service.Services;
using System.Reflection;

namespace AuthServer.API.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApiService(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return services;
        }
    }
}
