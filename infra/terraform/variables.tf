variable "prefix" {
  type    = string
  default = "ardent"
}

variable "location" {
  type    = string
  default = "West Europe"
}

variable "resource_group_name" {
  type    = string
  default = "${var.prefix}-rg"
}

variable "apim_name" {
  type    = string
  default = "${var.prefix}-apim"
}

variable "app_service_name" {
  type    = string
  default = "${var.prefix}-app"
}

variable "sku_name" {
  type    = string
  default = "Consumption"
}
