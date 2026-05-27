# ContentHub2SharePointSync

Production-ready Azure Functions-based **standalone DR-document sync pipeline** from Sitecore Content Hub to SharePoint Online.

## Architecture
- Timer-triggered Azure Function (`DrDocumentSync`) with weekly CRON schedule.
- Sitecore REST API client with pagination and retry.
- Metadata filter + transformation.
- SharePoint upsert through Microsoft Graph SDK.
- Incremental sync state stored in Azure Table Storage.
- Managed identity for Graph + Azure resources.
- Key Vault-backed secret loading.
- Structured telemetry to Application Insights.

## Deployment (Azure)
1. Provision infra (Bicep):
   ```bash
   az deployment group create -g <rg> -f infra/main.bicep -p functionAppName=<name> storageAccountName=<stg> appInsightsName=<ai> keyVaultName=<kv>
   ```
2. Assign system managed identity to Function App.
3. Grant identity permissions:
   - SharePoint/Graph app role permissions (Sites.ReadWrite.All).
   - Key Vault Secrets User RBAC.
   - Storage Table Data Contributor.
4. Configure application settings:
   - `SyncOptions:Schedule` (e.g. `0 0 2 * * 0` every Sunday 02:00 UTC)
   - `SitecoreOptions:*`
   - `SharePointOptions:*`
   - `StateStoreOptions:*`
   - `KeyVault:VaultUri`
5. Publish function app:
   ```bash
   func azure functionapp publish <function-app-name>
   ```

## Local notes
- Requires .NET 8 SDK and Azure Functions Core Tools v4.
- Unit test sample in `tests/DrDocumentSync.Tests`.
