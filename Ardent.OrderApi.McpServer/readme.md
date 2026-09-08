# Ardent Order MCP Server

A remotely hosted [Model Context Protocol](https://modelcontextprotocol.io/) server that exposes the
Order API to MCP clients (Claude Desktop, Claude Code, and anything else that speaks MCP).

It reuses the same clean-architecture layers as the REST API — MediatR requests against
`Ardent.OrderApi.Application`, backed by the Cosmos repository — so tools and HTTP endpoints
never drift apart.

## Transport

Streamable HTTP on `POST/GET/DELETE /mcp`, configured stateless so requests are self-contained
and the app can scale out. `GET /health` is a plain liveness probe.

## Tools

| Tool | Description |
| --- | --- |
| `get_order` | Reads a single order by order id + customer id (orders are partitioned by customer). |
| `get_customer_orders` | Reads every order for a customer. |
| `create_order` | Creates an order from a customer id and a list of products. |

## Configuration

| Setting | Purpose |
| --- | --- |
| `CosmosDb:AccountEndpoint` / `AccountKey` / `DatabaseId` / `ContainerId` | Order store, same values as the REST API. |
| `Mcp:RequireAuthorization` | `false` (default) leaves `/mcp` open — only appropriate locally or behind a private network. `true` requires a bearer token. |
| `Mcp:ResourceUri` | Public URL of this server, e.g. `https://ardent-order-mcp.azurewebsites.net/mcp`. Required when authorization is enabled; it is published in the protected resource metadata. |
| `Keycloak:Authority` / `Audience` / `RequireHttpsMetadata` | Token issuer, shared with the REST API. |

With `Mcp:RequireAuthorization` set to `true` the server validates JWT bearer tokens and serves
`/.well-known/oauth-protected-resource`, which is how a spec-compliant MCP client discovers the
authorization server and starts an OAuth flow.

Never commit Cosmos keys. Locally use `dotnet user-secrets`; in Azure use App Service application
settings (`CosmosDb__AccountEndpoint`, `CosmosDb__AccountKey`, ...).

## Running locally

```bash
dotnet run --project Ardent.OrderApi.McpServer
# http://localhost:5280/mcp
```

Smoke test the handshake:

```bash
curl -X POST http://localhost:5280/mcp \
  -H "Content-Type: application/json" \
  -H "Accept: application/json, text/event-stream" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'
```

## Connecting clients

### Claude Code

```bash
claude mcp add --transport http ardent-orders https://<your-host>/mcp
# with a bearer token:
claude mcp add --transport http ardent-orders https://<your-host>/mcp \
  --header "Authorization: Bearer <token>"
```

### Claude Desktop

Settings → Connectors → Add custom connector → paste `https://<your-host>/mcp`.
Custom connectors require a public HTTPS URL; if the server has authorization enabled, Desktop
runs the OAuth flow discovered from the protected resource metadata.

For a local server (no public URL), point Desktop at it through the stdio bridge in
`claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "ardent-orders": {
      "command": "npx",
      "args": ["-y", "mcp-remote", "http://localhost:5280/mcp"]
    }
  }
}
```

### Other MCP clients

Any client with Streamable HTTP support connects to `https://<your-host>/mcp` directly. Clients
that only speak stdio can use the `mcp-remote` bridge shown above.

## Hosting on Azure

Deploys like any ASP.NET Core app — App Service or Container Apps. Two requirements:

- HTTPS with a public hostname (MCP clients will not connect over plain HTTP remotely).
- Response buffering disabled / HTTP/1.1 streaming allowed, so Server-Sent Events flow through.
  App Service and Container Apps both do this by default; if a reverse proxy or APIM sits in
  front, make sure it does not buffer `text/event-stream` responses.

Stateless mode means no session affinity is needed when scaling to multiple instances.
