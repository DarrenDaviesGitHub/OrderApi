terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 5.0.0, < 6.0.0"
    }
  }
}

resource "azurerm_cosmosdb_account" "this" {
  name                = var.account_name
  resource_group_name = var.resource_group_name
  location            = var.location

  offer_type = "Standard"
  kind       = "GlobalDocumentDB"
  free_tier_enabled = true
  local_authentication_enabled = false
  minimal_tls_version          = "Tls12"

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = var.location
    failover_priority = 0
  }

  tags = var.tags
}

resource "azurerm_cosmosdb_sql_database" "this" {
  name                = var.database_name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.this.name

  dynamic "autoscale_settings" {
    # Serverless accounts must not declare throughput at all.
    for_each = var.serverless ? [] : [1]
    content {
      max_throughput = var.max_throughput
    }
  }
}

resource "azurerm_cosmosdb_sql_container" "this" {
  for_each = { for c in var.containers : c.name => c }

  name                = each.value.name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.this.name
  database_name       = azurerm_cosmosdb_sql_database.this.name

  partition_key_paths   = [each.value.partition_key_path]
  partition_key_version = 2
}