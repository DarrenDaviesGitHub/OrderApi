variable "account_name" {
  description = "Globally unique Cosmos DB account name."
  type        = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "serverless" {
  description = "Serverless billing (dev) instead of provisioned throughput (prod)."
  type        = bool
  default     = false
}

variable "max_throughput" {
  description = "Autoscale ceiling in RU/s, shared across the database. Ignored when serverless."
  type        = number
  default     = 1000
}

variable "database_name" {
  type = string
}

variable "containers" {
  description = "Containers to create."
  type = list(object({
    name                = string
    partition_key_path  = string
  }))
}

variable "tags" {
  type    = map(string)
  default = {}
}