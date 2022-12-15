using AuthServer.API.Modules;
using AuthServer.Core.Models;
using AuthServer.Data;
using AuthServer.Service.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddServiceDependencies();
//builder.Services.AddDataDependencies(builder.Configuration);

builder.Services.AddIdentity<UserApp, IdentityRole>(opt =>
{
	opt.User.RequireUniqueEmail = true;
	opt.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt =>
{
	opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
	var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
	opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
	{
		ValidIssuer = tokenOptions.Issuer,
		ValidAudience = tokenOptions.Audience.FirstOrDefault(),
		IssuerSigningKey = SignService.GetSymetricSecurityKey(tokenOptions.SecurityKey),

		ValidateLifetime = true,
		ValidateIssuer = true,
		ValidateIssuerSigningKey = true,
		ValidateAudience = true,
		ClockSkew = TimeSpan.Zero
	};
});
//builder.Services.AddDbContext<AppDbContext>(x =>
//{
//	x.UseSqlServer(builder.Configuration.GetConnectionString("AuthServerDb"), option =>
//	{
//		option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext))!.GetName().Name);
//	});
//});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(cb => cb.RegisterModule(new RepoServiceModule(builder.Configuration)));

var app = builder.Build();

//builder.Services.Configure<CustomTokenOptions>(app.Configuration.GetSection("TokenOptions"));
//builder.Services.Configure<List<Client>>(app.Configuration.GetSection("Clients"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
