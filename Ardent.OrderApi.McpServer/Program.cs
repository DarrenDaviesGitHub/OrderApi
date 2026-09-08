using Ardent.OrderApi.Application;
using Ardent.OrderApi.Application.Interfaces;
using Ardent.OrderApi.Application.MappingProfiles;
using Ardent.OrderApi.Infrastructure.Cosmos.Configuration;
using Ardent.OrderApi.Infrastructure.Cosmos.Repository;
using Ardent.OrderApi.McpServer.Tools;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using ModelContextProtocol.AspNetCore.Authentication;
using ModelContextProtocol.Authentication;
using ModelContextProtocol.Protocol;

var builder = WebApplication.CreateBuilder(args);

var mcpConfiguration = builder.Configuration.GetSection("Mcp");
var keycloakConfiguration = builder.Configuration.GetSection("Keycloak");
var requireAuthorization = mcpConfiguration.GetValue("RequireAuthorization", false);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<OrderProfile>();
    cfg.AddProfile<ProductProfile>();
});

builder.Services.Configure<CosmosDbOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(options.AccountEndpoint, options.AccountKey);
});

builder.Services.AddScoped<ICosmosOrderRepository, CosmosOrderRepository>();

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new Implementation
        {
            Name = "ardent-order-api",
            Title = "Ardent Order API",
            Version = "1.0.0"
        };
        options.ServerInstructions =
            "Tools for reading and creating customer orders held in the Ardent Cosmos DB order container. " +
            "Orders are partitioned by customer, so a customer id is required alongside an order id.";
    })

    // Streamable HTTP is the transport remote MCP clients (Claude Desktop, Claude Code) connect over.
    // Stateless mode keeps every request self contained so the server can scale out behind App Service.
    .WithHttpTransport(options => options.Stateless = true)
    .WithTools<OrderTools>();

if (requireAuthorization)
{
    // Bearer tokens are validated against the same Keycloak realm as the REST API, and the MCP
    // scheme publishes /.well-known/oauth-protected-resource so clients can discover it.
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = keycloakConfiguration["Authority"];
            options.Audience = keycloakConfiguration["Audience"];
            options.RequireHttpsMetadata = bool.Parse(keycloakConfiguration["RequireHttpsMetadata"] ?? "false");
        })
        .AddMcp(options =>
        {
            options.ResourceMetadata = new ProtectedResourceMetadata
            {
                Resource = mcpConfiguration["ResourceUri"]
                    ?? throw new InvalidOperationException("Mcp:ResourceUri must be set when Mcp:RequireAuthorization is true."),
                AuthorizationServers = { keycloakConfiguration["Authority"]! },
                ScopesSupported = ["openid", "profile"]
            };
        });

    builder.Services.AddAuthorization();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

if (requireAuthorization)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

var mcpEndpoints = app.MapMcp("/mcp");

if (requireAuthorization)
{
    mcpEndpoints.RequireAuthorization();
}

app.Run();
