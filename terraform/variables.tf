variable "project" {
  description = "Project name"
  type        = string
  default     = "reelbuy"
}

variable "environment" {
  description = "Environment (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastus"
}

variable "address_space" {
  description = "VNet address space"
  type        = list(string)
  default     = ["10.0.0.0/16"]
}

variable "subnet_prefixes" {
  description = "Subnet address prefixes"
  type        = map(string)
  default = {
    app_gateway = "10.0.1.0/24"
    app_service = "10.0.2.0/24"
    integration = "10.0.3.0/24"
    database   = "10.0.4.0/24"
    private_endpoints = "10.0.5.0/24"
  }
}

variable "app_gateway_capacity" {
  description = "Number of Application Gateway capacity units"
  type        = number
  default     = 2
}

variable "app_gateway_sku" {
  description = "SKU for Application Gateway"
  type        = map(string)
  default = {
    name = "Standard_v2"
    tier = "Standard_v2"
  }
}

variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default = {
    environment = "dev"
    managed_by  = "terraform"
    project     = "reelbuy"
  }
} 