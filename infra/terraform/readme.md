This Terraform config deploys:
- Resource Group
- App Service Plan (Consumption)
- App Service
- API Management (Consumption)
- API imported from the App Service swagger

Usage:
1. Install Terraform and Azure CLI
2. Login: `az login`
3. Initialize: `terraform init` in `infra/terraform`
4. Plan: `terraform plan -var 'location=<region>'`
5. Apply: `terraform apply -var 'location=<region>'`

Notes:
- App Service is configured to run from package; update deployment pipeline to push package.
- APIM in Consumption SKU might have some limitations around custom domains and developer portal.
