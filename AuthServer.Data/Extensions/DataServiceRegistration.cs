using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Data.Extensions
{
	public static class DataServiceRegistration
	{
		public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(configuration.GetConnectionString("AuthServerDb"));
			});

			return services;
		}
	}
}
