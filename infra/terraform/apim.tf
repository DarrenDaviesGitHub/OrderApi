resource "azurerm_api_management" "apim" {
  name                = var.apim_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  publisher_name  = "Ardent"
  publisher_email = "devops@ardent.local"

  sku_name = var.sku_name

  gateway_regional_url_enabled = true
}

resource "null_resource" "wait_for_swagger" {
  provisioner "local-exec" {
    command = <<EOT
for i in {1..30}; do
  if curl -fsS https://${azurerm_app_service.app.default_site_hostname}/swagger/v1/swagger.json >/dev/null 2>&1; then
    exit 0
  fi
  sleep 5
done
exit 1
EOT
  }

  depends_on = [azurerm_app_service.app]
}

resource "azurerm_api_management_api" "api" {
  name                = "orders-api"
  resource_group_name = azurerm_resource_group.rg.name
  api_management_name = azurerm_api_management.apim.name
  revision            = "1"
  display_name        = "Orders API"
  path                = "orders"
  protocols           = ["https"]

  import {
    content_format = "openapi-link"
    content_value  = "https://${azurerm_app_service.app.default_site_hostname}/swagger/v1/swagger.json"
  }

  depends_on = [null_resource.wait_for_swagger]
}

resource "azurerm_api_management_api_operation" "get_orders" {
  operation_id        = "get-orders"
  api_name            = azurerm_api_management_api.api.name
  api_management_name = azurerm_api_management.apim.name
  resource_group_name = azurerm_resource_group.rg.name
  display_name        = "Get Orders"
  method              = "GET"
  url_template        = "/"
}

resource "azurerm_api_management_api_operation" "get_order_by_id" {
  operation_id        = "get-order-by-id"
  api_name            = azurerm_api_management_api.api.name
  api_management_name = azurerm_api_management.apim.name
  resource_group_name = azurerm_resource_group.rg.name
  display_name        = "Get Order by Id"
  method              = "GET"
  url_template        = "/{id}"
}
