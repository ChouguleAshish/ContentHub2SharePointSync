param location string = resourceGroup().location
param functionAppName string
param storageAccountName string
param appInsightsName string
param keyVaultName string
param tableName string = 'DrSyncState'

resource sa 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
}

resource ai 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: { Application_Type: 'web' }
}

resource kv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: { family: 'A', name: 'standard' }
    enableRbacAuthorization: true
  }
}

output storageAccountId string = sa.id
output applicationInsightsConnectionString string = ai.properties.ConnectionString
output keyVaultUri string = kv.properties.vaultUri
