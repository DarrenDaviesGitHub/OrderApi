using Ardent.Infrastructure.Cosmos.Configuration;
using Ardent.Infrastructure.Cosmos.Interfaces;
using Ardent.Infrastructure.Cosmos.Repository;
using Ardent.OrderApi.Application;
using Ardent.OrderApi.Middleware;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var keycloakConfiguration = builder.Configuration.GetSection("Keycloak");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
builder.Services.AddAutoMapper(cfg => { });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakConfiguration["Authority"];
        options.Audience = keycloakConfiguration["Audience"];
        options.RequireHttpsMetadata = bool.Parse(keycloakConfiguration["RequireHttpsMetadata"] ?? "false");
    });
builder.Services.Configure<CosmosDbOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(options.AccountEndpoint, options.AccountKey);
});

builder.Services.AddScoped<ICosmosOrderRepository, CosmosOrderRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

app.MapOpenApi();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
