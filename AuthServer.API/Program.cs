using AuthServer.API.Extensions;
using AuthServer.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceRegistration(builder.Configuration);
builder.Services.AddApiService();


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
