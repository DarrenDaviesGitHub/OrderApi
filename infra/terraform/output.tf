output "apim_management_url" {
  value = azurerm_api_management.apim.gateway_url
}

output "apim_publisher_email" {
  value = azurerm_api_management.apim.publisher_email
}
