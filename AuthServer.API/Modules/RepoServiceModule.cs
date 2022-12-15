using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Autofac;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.API.Modules
{
	public class RepoServiceModule:Module
	{
		private readonly IConfiguration configuration;

		public RepoServiceModule(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterGeneric(typeof(GenericRepository<>))
				.As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(GenericService<,>))
				.As(typeof(IGenericService<,>)).InstancePerLifetimeScope();

			builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().SingleInstance();
			builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().SingleInstance();	
			builder.RegisterType<TokenService>().As<ITokenService>().SingleInstance();
			builder.RegisterType<UserService>().As<IUserService>().SingleInstance();


			builder.Register(x =>
			{
				var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
				optionsBuilder.UseSqlServer(configuration.GetConnectionString("AuthServerDb"));
				return new AppDbContext(optionsBuilder.Options);
			}).InstancePerLifetimeScope();

			
			base.Load(builder);
		}
	}
}
