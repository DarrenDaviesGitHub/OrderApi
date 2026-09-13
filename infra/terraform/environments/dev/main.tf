terraform {
  required_version = ">= 1.9.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 5.5"
    }
  }

  backend "azurerm" {
    resource_group_name  = "rg-tfstate-dev-uks"
    storage_account_name = "stardenttfstatedevuks01"
    container_name       = "tfstate"
    key                  = "dev.tfstate"
    use_azuread_auth     = true
    use_cli          = true
  }
}

provider "azurerm" {
  resource_providers_to_register = ["Microsoft.Web", "Microsoft.DocumentDB"]
  features {}
}

data "azurerm_subscription" "current" {}

locals {
  workload    = "orderapi"
  environment = "dev"
  location    = "uksouth"

  name = "${local.workload}-${local.environment}-uks"
  unique = substr(sha1("${data.azurerm_subscription.current.subscription_id}-${local.name}"), 0, 6)

  tags = {
    workload    = local.workload
    environment = local.environment
    managed_by  = "terraform"
  }
}

resource "azurerm_resource_group" "main" {
  name     = "rg-${local.name}"
  location = local.location
  tags     = local.tags
}

module "cosmosdb" {
  source = "../../modules/cosmosdb"

  account_name        = "cosmos-${local.name}-${local.unique}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  # Serverless bills per request
  serverless = true

  database_name = "ardentdb"
  containers = [
    { name = "orders", partition_key_path = "/customerId" },
  ]

  tags = local.tags
}

module "app_service" {
  source = "../../modules/app-service"

  name                = local.name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  sku_name               = "B1"
  aspnetcore_environment = "Development"

  cosmosdb_endpoint      = module.cosmosdb.endpoint
  cosmosdb_database_name = module.cosmosdb.database_name

  tags = local.tags
}

resource "azurerm_cosmosdb_sql_role_assignment" "api" {
  resource_group_name = azurerm_resource_group.main.name
  account_name        = module.cosmosdb.account_name
  principal_id        = module.app_service.principal_id
  scope               = module.cosmosdb.account_id
  role_definition_id = "${module.cosmosdb.account_id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
}

output "api_url" {
  value = "https://${module.app_service.default_hostname}"
}

output "web_app_name" {
  value = module.app_service.name
}