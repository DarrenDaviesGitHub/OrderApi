using Ardent.Infrastructure.Cosmos.Configuration;
using Ardent.Infrastructure.Cosmos.Interfaces;
using Ardent.Infrastructure.Cosmos.Repository;
using Ardent.OrderApi.Middleware;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.Configure<CosmosDbOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(options.AccountEndpoint, options.AccountKey);
});

builder.Services.AddScoped<ICosmosOrderRepository, CosmosOrderRepository>();

var app = builder.Build();
app.MapOpenApi();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
