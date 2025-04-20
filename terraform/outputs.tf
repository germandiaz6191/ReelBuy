output "resource_group_name" {
  description = "The name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "virtual_network_name" {
  description = "The name of the virtual network"
  value       = azurerm_virtual_network.main.name
}

output "virtual_network_id" {
  description = "The ID of the virtual network"
  value       = azurerm_virtual_network.main.id
}

output "app_gateway_subnet_id" {
  description = "The ID of the Application Gateway subnet"
  value       = azurerm_subnet.app_gateway.id
}

output "app_service_subnet_id" {
  description = "The ID of the App Service subnet"
  value       = azurerm_subnet.app_service.id
}

output "integration_subnet_id" {
  description = "The ID of the integration subnet"
  value       = azurerm_subnet.integration.id
}

output "database_subnet_id" {
  description = "The ID of the database subnet"
  value       = azurerm_subnet.database.id
}

output "private_endpoints_subnet_id" {
  description = "The ID of the private endpoints subnet"
  value       = azurerm_subnet.private_endpoints.id
}

output "app_gateway_public_ip" {
  description = "The public IP of the Application Gateway"
  value       = azurerm_public_ip.app_gateway.ip_address
} 