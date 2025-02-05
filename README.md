**Using User Assigned MI to access keyvault**
az identity show --resource-group defaultresourcegroup-eau --name azure-sb-topic-output-binding-uami --query id -o tsv

az functionapp update --resource-group defaultresourcegroup-eau --name azure-sb-topic-output-binding --set keyVaultReferenceIdentity=/subscriptions/8dc3aa21-5fcc-4c2e-837d-cf3c9fea564f/resourcegroups/DefaultResourceGroup-EAU/providers/Microsoft.ManagedIdentity/userAssignedIdentities/azure-sb-topic-output-binding-uami
