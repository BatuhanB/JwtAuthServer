using AuthServer.Core.Services;
using AuthServer.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace AuthServer.Service.Extensions
{
	public static class ServiceRegistration
	{
		public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
		{
			var assemebly = Assembly.GetExecutingAssembly();
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
			services.AddAutoMapper(assemebly);
			return services;
		}
	}
}
