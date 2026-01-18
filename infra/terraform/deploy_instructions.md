Recommended deployment steps:

1. Build and publish your API to a zip package:
   dotnet publish -c Release -o ./publish
   cd publish
   zip -r ../app.zip .

2. Deploy the zip to the App Service (using Azure CLI):
   az webapp deployment source config-zip --resource-group <rg> --name <app_service_name> --src app.zip

3. Run Terraform from infra/terraform:
   terraform init
   terraform plan -var 'location=West Europe'
   terraform apply -var 'location=West Europe'

Note: You can combine steps 2 and 3 in a CI pipeline where the app is deployed before Terraform runs, or add an automation step that deploys the package and then runs Terraform in the same job.
