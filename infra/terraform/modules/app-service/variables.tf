variable "name" {
  description = "Web app name"
  type        = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "sku_name" {
  description = "App Service Plan SKU, e.g. B1 or P1v3."
  type        = string
}

variable "dotnet_version" {
  type    = string
  default = "10.0"
}

variable "aspnetcore_environment" {
  type    = string
  default = "Production"
}

variable "cosmosdb_endpoint" {
  type = string
}

variable "cosmosdb_database_name" {
  type = string
}

variable "tags" {
  type    = map(string)
  default = {}
}