@description('Cosmos DB account name to create the database under.')
param cosmosAccountName string

@description('Name of the SQL database.')
param databaseName string

@description('Container definitions for this database.')
param containers { name: string, partitionKeyPath: string }[]

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-11-15' existing = {
  name: cosmosAccountName
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-11-15' = {
  parent: cosmosAccount
  name: databaseName
  properties: {
    resource: { id: databaseName }
  }
}

resource sqlContainers 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-11-15' = [
  for c in containers: {
    parent: database
    name: c.name
    properties: {
      resource: {
        id: c.name
        partitionKey: {
          paths: [c.partitionKeyPath]
          kind: 'Hash'
          version: 2
        }
      }
    }
  }
]
