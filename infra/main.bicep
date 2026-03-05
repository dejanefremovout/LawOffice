targetScope = 'resourceGroup'

// ─── Parameters ─────────────────────────────────────────────────────────────────

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Environment short name used for resource naming.')
@allowed(['dev', 'test', 'prod'])
param environmentName string

@description('Common tags applied to all resources.')
param tags object = {}

@description('API Management publisher email address.')
param apimPublisherEmail string

@description('Entra ID OpenID Configuration URL for APIM JWT validation. Leave empty to skip JWT validation.')
param jwtOpenIdConfigUrl string = ''

@description('Entra ID audience (application client ID) for JWT validation.')
param jwtAudience string = ''

@description('Entra ID token issuer URL for JWT validation.')
param jwtIssuer string = ''

@description('Storage account name. Default: stlawoffice<env>.')
param storageAccountName string = 'stlawoffice${environmentName}'

@description('Cosmos DB account name. Default: cos-lawoffice-<env>.')
param cosmosAccountName string = 'cos-lawoffice-officemanagement-${environmentName}'

@description('Static Web App name. Default: swa-lawoffice-portal-<env>.')
param staticWebAppName string = 'swa-lawoffice-portal-${environmentName}'

@description('Repository URL for SWA GitHub integration. Leave empty to skip.')
param staticWebAppRepositoryUrl string = ''

@description('Repository branch for SWA.')
param staticWebAppBranch string = ''

@description('Wire APIM backends to Function App host keys. Set to false on first deploy (before code is published) to avoid host runtime errors.')
param configureApimBackends bool = true

// ─── Variables ──────────────────────────────────────────────────────────────────

var prefix = 'lawoffice'

// Microservice definitions – each entry drives Function App, APIM API, and backend creation
var microservices = [
  { key: 'casemanagement', displayName: 'CaseManagement', apiPath: 'case', needsBlobStorage: true }
  { key: 'officemanagement', displayName: 'OfficeManagement', apiPath: 'office', needsBlobStorage: false }
  { key: 'partymanagement', displayName: 'PartyManagement', apiPath: 'party', needsBlobStorage: false }
]

// APIM operation definitions mapped to each microservice API
var apimOperations = [
  // CaseManagement
  { serviceIndex: 0, operationId: 'get-case', displayName: 'Get Case', method: 'GET', urlTemplate: '/case/{caseId}', pathParameters: ['caseId'] }
  { serviceIndex: 0, operationId: 'get-all-cases', displayName: 'Get All Cases', method: 'GET', urlTemplate: '/case', pathParameters: [] }
  { serviceIndex: 0, operationId: 'post-case', displayName: 'Create Case', method: 'POST', urlTemplate: '/case', pathParameters: [] }
  { serviceIndex: 0, operationId: 'put-case', displayName: 'Update Case', method: 'PUT', urlTemplate: '/case', pathParameters: [] }
  { serviceIndex: 0, operationId: 'delete-case', displayName: 'Delete Case', method: 'DELETE', urlTemplate: '/case/{caseId}', pathParameters: ['caseId'] }
  { serviceIndex: 0, operationId: 'get-cases-count', displayName: 'Get Cases Count', method: 'GET', urlTemplate: '/cases/count', pathParameters: [] }
  { serviceIndex: 0, operationId: 'get-last-cases', displayName: 'Get Last Cases', method: 'GET', urlTemplate: '/cases/last/{count}', pathParameters: ['count'] }
  { serviceIndex: 0, operationId: 'get-cases-with-hearings', displayName: 'Get Cases With Hearings', method: 'GET', urlTemplate: '/cases/hearings/{count}', pathParameters: ['count'] }
  { serviceIndex: 0, operationId: 'get-document-file', displayName: 'Get Document File', method: 'GET', urlTemplate: '/documentFile/{documentFileId}', pathParameters: ['documentFileId'] }
  { serviceIndex: 0, operationId: 'get-all-document-files', displayName: 'Get All Document Files', method: 'GET', urlTemplate: '/documentFile/case/{caseId}', pathParameters: ['caseId'] }
  { serviceIndex: 0, operationId: 'post-document-file', displayName: 'Create Document File', method: 'POST', urlTemplate: '/documentFile', pathParameters: [] }
  { serviceIndex: 0, operationId: 'put-document-file', displayName: 'Update Document File', method: 'PUT', urlTemplate: '/documentFile', pathParameters: [] }
  { serviceIndex: 0, operationId: 'delete-document-file', displayName: 'Delete Document File', method: 'DELETE', urlTemplate: '/documentFile/{documentFileId}', pathParameters: ['documentFileId'] }
  { serviceIndex: 0, operationId: 'get-hearing', displayName: 'Get Hearing', method: 'GET', urlTemplate: '/hearing/{hearingId}', pathParameters: ['hearingId'] }
  { serviceIndex: 0, operationId: 'get-all-hearings', displayName: 'Get All Hearings', method: 'GET', urlTemplate: '/hearing/case/{caseId}', pathParameters: ['caseId'] }
  { serviceIndex: 0, operationId: 'post-hearing', displayName: 'Create Hearing', method: 'POST', urlTemplate: '/hearing', pathParameters: [] }
  { serviceIndex: 0, operationId: 'put-hearing', displayName: 'Update Hearing', method: 'PUT', urlTemplate: '/hearing', pathParameters: [] }
  { serviceIndex: 0, operationId: 'delete-hearing', displayName: 'Delete Hearing', method: 'DELETE', urlTemplate: '/hearing/{hearingId}', pathParameters: ['hearingId'] }

  // OfficeManagement
  { serviceIndex: 1, operationId: 'get-lawyer', displayName: 'Get Lawyer', method: 'GET', urlTemplate: '/lawyer/{lawyerId}', pathParameters: ['lawyerId'] }
  { serviceIndex: 1, operationId: 'get-all-lawyers', displayName: 'Get All Lawyers', method: 'GET', urlTemplate: '/lawyer', pathParameters: [] }
  { serviceIndex: 1, operationId: 'post-lawyer', displayName: 'Create Lawyer', method: 'POST', urlTemplate: '/lawyer', pathParameters: [] }
  { serviceIndex: 1, operationId: 'put-lawyer', displayName: 'Update Lawyer', method: 'PUT', urlTemplate: '/lawyer', pathParameters: [] }
  { serviceIndex: 1, operationId: 'get-office', displayName: 'Get Office', method: 'GET', urlTemplate: '/office', pathParameters: [] }
  { serviceIndex: 1, operationId: 'put-office', displayName: 'Update Office', method: 'PUT', urlTemplate: '/office', pathParameters: [] }

  // PartyManagement
  { serviceIndex: 2, operationId: 'get-party-count', displayName: 'Get Party Count', method: 'GET', urlTemplate: '/party/count', pathParameters: [] }
  { serviceIndex: 2, operationId: 'get-opposing-party', displayName: 'Get Opposing Party', method: 'GET', urlTemplate: '/opposingParty/{opposingPartyId}', pathParameters: ['opposingPartyId'] }
  { serviceIndex: 2, operationId: 'get-all-opposing-parties', displayName: 'Get All Opposing Parties', method: 'GET', urlTemplate: '/opposingParty', pathParameters: [] }
  { serviceIndex: 2, operationId: 'post-opposing-party', displayName: 'Create Opposing Party', method: 'POST', urlTemplate: '/opposingParty', pathParameters: [] }
  { serviceIndex: 2, operationId: 'put-opposing-party', displayName: 'Update Opposing Party', method: 'PUT', urlTemplate: '/opposingParty', pathParameters: [] }
  { serviceIndex: 2, operationId: 'get-client', displayName: 'Get Client', method: 'GET', urlTemplate: '/client/{clientId}', pathParameters: ['clientId'] }
  { serviceIndex: 2, operationId: 'get-all-clients', displayName: 'Get All Clients', method: 'GET', urlTemplate: '/client', pathParameters: [] }
  { serviceIndex: 2, operationId: 'post-client', displayName: 'Create Client', method: 'POST', urlTemplate: '/client', pathParameters: [] }
  { serviceIndex: 2, operationId: 'put-client', displayName: 'Update Client', method: 'PUT', urlTemplate: '/client', pathParameters: [] }
]

// Cosmos DB database and container layout (mirrors microservice boundaries)
var cosmosDatabases = [
  {
    name: 'casemanagement'
    containers: [
      { name: 'cases', partitionKeyPath: '/officeId' }
      { name: 'documentfiles', partitionKeyPath: '/officeId' }
      { name: 'hearings', partitionKeyPath: '/officeId' }
    ]
  }
  {
    name: 'officemanagement'
    containers: [
      { name: 'lawyers', partitionKeyPath: '/officeId' }
      { name: 'offices', partitionKeyPath: '/id' }
    ]
  }
  {
    name: 'partymanagement'
    containers: [
      { name: 'clients', partitionKeyPath: '/officeId' }
      { name: 'opposingparties', partitionKeyPath: '/officeId' }
    ]
  }
]

// ─── Storage ────────────────────────────────────────────────────────────────────

resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

resource storageBlobService 'Microsoft.Storage/storageAccounts/blobServices@2025-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: [
        {
          allowedOrigins: [
            'https://${staticWebApp.properties.defaultHostname}'
          ]
          allowedMethods: [
            'GET'
            'OPTIONS'
            'PUT'
          ]
          allowedHeaders: [
            'x-ms-blob-type'
            'content-type'
          ]
          exposedHeaders: [
            'x-ms-request-id'
            'x-ms-version'
          ]
          maxAgeInSeconds: 3600
        }
      ]
    }
  }
}

// ─── Cosmos DB (NoSQL, Serverless) ──────────────────────────────────────────────

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-11-15' = {
  name: cosmosAccountName
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    publicNetworkAccess: 'Enabled'
    minimalTlsVersion: 'Tls12'
    enableAutomaticFailover: true
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      { name: 'EnableServerless' }
    ]
  }
}

module cosmosSqlDatabases 'modules/cosmos-sql-database.bicep' = [
  for db in cosmosDatabases: {
    params: {
      cosmosAccountName: cosmosAccount.name
      databaseName: db.name
      containers: db.containers
    }
  }
]

// ─── App Service Plan (Consumption) ─────────────────────────────────────────────

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: 'asp-${prefix}-${environmentName}'
  location: location
  tags: tags
  kind: 'functionapp'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

// ─── Function Apps ──────────────────────────────────────────────────────────────

resource funcApps 'Microsoft.Web/sites@2024-11-01' = [
  for (svc, i) in microservices: {
    name: 'func-${prefix}-${svc.key}-${environmentName}'
    location: location
    tags: tags
    kind: 'functionapp'
    identity: { type: 'SystemAssigned' }
    properties: {
      serverFarmId: appServicePlan.id
      httpsOnly: true
      clientAffinityEnabled: false
      publicNetworkAccess: 'Enabled'
      keyVaultReferenceIdentity: 'SystemAssigned'
      siteConfig: {
        alwaysOn: false
        http20Enabled: false
        minTlsVersion: '1.2'
        scmMinTlsVersion: '1.2'
        ftpsState: 'FtpsOnly'
        functionAppScaleLimit: 200
        minimumElasticInstanceCount: 0
        appSettings: concat(
          [
            {
              name: 'AzureWebJobsStorage'
              value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
            }
            {
              name: 'FUNCTIONS_EXTENSION_VERSION'
              value: '~4'
            }
            {
              name: 'FUNCTIONS_WORKER_RUNTIME'
              value: 'dotnet-isolated'
            }
            {
              name: 'WEBSITE_RUN_FROM_PACKAGE'
              value: '1'
            }
            {
              name: 'CosmosSettings:ConnectionString'
              value: cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString
            }
          ],
          svc.needsBlobStorage
            ? [
                {
                  name: 'BlobSettings:ConnectionString'
                  value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
                }
              ]
            : []
        )
      }
    }
  }
]

resource funcFtpPolicies 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = [
  for (svc, i) in microservices: {
    parent: funcApps[i]
    name: 'ftp'
    properties: { allow: false }
  }
]

resource funcScmPolicies 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = [
  for (svc, i) in microservices: {
    parent: funcApps[i]
    name: 'scm'
    properties: { allow: false }
  }
]

// ─── Static Web App ─────────────────────────────────────────────────────────────

resource staticWebApp 'Microsoft.Web/staticSites@2024-11-01' = {
  name: staticWebAppName
  location: location
  tags: tags
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    repositoryUrl: empty(staticWebAppRepositoryUrl) ? null : staticWebAppRepositoryUrl
    branch: empty(staticWebAppBranch) ? null : staticWebAppBranch
    provider: !empty(staticWebAppRepositoryUrl) ? 'GitHub' : null
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    enterpriseGradeCdnStatus: 'Disabled'
  }
}

// ─── API Management ─────────────────────────────────────────────────────────────

resource apim 'Microsoft.ApiManagement/service@2024-06-01-preview' = {
  name: 'apim-${prefix}-${environmentName}'
  location: location
  tags: tags
  sku: {
    name: 'Consumption'
    capacity: 0
  }
  identity: { type: 'SystemAssigned' }
  properties: {
    publisherName: 'LawOffice'
    publisherEmail: apimPublisherEmail
    notificationSenderEmail: 'apimgmt-noreply@mail.windowsazure.com'
    publicNetworkAccess: 'Enabled'
    virtualNetworkType: 'None'
    disableGateway: false
    customProperties: {
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls10': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls11': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls10': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls11': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Ssl30': 'False'
    }
  }
}

// APIM global policy – CORS (referencing SWA origin) + optional JWT validation
var jwtPolicyFragment = !empty(jwtOpenIdConfigUrl)
  ? '<validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized. Access token is missing or invalid."><openid-config url="${jwtOpenIdConfigUrl}" /><audiences><audience>${jwtAudience}</audience></audiences><issuers><issuer>${jwtIssuer}</issuer></issuers></validate-jwt>'
  : ''

var apimPolicyXml = replace(
  replace(
    loadTextContent('policies/apim-global-policy.xml'),
    '{{SWA_ORIGIN}}',
    'https://${staticWebApp.properties.defaultHostname}'
  ),
  '{{JWT_SECTION}}',
  jwtPolicyFragment
)

resource apimGlobalPolicy 'Microsoft.ApiManagement/service/policies@2024-06-01-preview' = {
  parent: apim
  name: 'policy'
  properties: {
    format: 'xml'
    value: apimPolicyXml
  }
}

// APIM API definitions
resource apimApis 'Microsoft.ApiManagement/service/apis@2024-06-01-preview' = [
  for svc in microservices: {
    parent: apim
    name: svc.displayName
    properties: {
      displayName: svc.displayName
      apiRevision: '1'
      description: 'Managed via Bicep for "func-${prefix}-${svc.key}-${environmentName}" Function App'
      subscriptionRequired: false
      path: svc.apiPath
      protocols: ['https']
      isCurrent: true
    }
  }
]

// APIM API operations managed as code (mirrors Function HTTP triggers)
resource apimApiOperations 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = [
  for op in apimOperations: {
    parent: apimApis[op.serviceIndex]
    name: op.operationId
    properties: {
      displayName: op.displayName
      method: op.method
      urlTemplate: op.urlTemplate
      templateParameters: [
        for paramName in op.pathParameters: {
          name: paramName
          description: '${paramName} path parameter'
          type: 'string'
          required: true
        }
      ]
      responses: [
        {
          statusCode: 200
        }
      ]
    }
  }
]

// APIM named values holding Function App host keys (requires deployed code)
resource apimNamedValues 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = [
  for (svc, i) in microservices: if (configureApimBackends) {
    parent: apim
    name: 'func-${prefix}-${svc.key}-${environmentName}-key'
    properties: {
      displayName: 'func-${prefix}-${svc.key}-${environmentName}-key'
      tags: ['key', 'function', 'auto']
      secret: true
      value: listKeys('${funcApps[i].id}/host/default', '2024-11-01').functionKeys.default
    }
  }
]

// APIM backends linking to Function Apps (requires deployed code)
resource apimBackends 'Microsoft.ApiManagement/service/backends@2024-06-01-preview' = [
  for (svc, i) in microservices: if (configureApimBackends) {
    parent: apim
    name: 'func-${prefix}-${svc.key}-${environmentName}'
    properties: {
      description: 'func-${prefix}-${svc.key}-${environmentName}'
      url: 'https://${funcApps[i].properties.defaultHostName}/api'
      protocol: 'http'
      resourceId: uri(environment().resourceManager, funcApps[i].id)
      credentials: {
        header: {
          'x-functions-key': ['{{func-${prefix}-${svc.key}-${environmentName}-key}}']
        }
      }
    }
    dependsOn: [apimNamedValues[i]]
  }
]

// APIM API policy to route each API to its Function backend
resource apimApiPolicies 'Microsoft.ApiManagement/service/apis/policies@2024-06-01-preview' = [
  for (svc, i) in microservices: if (configureApimBackends) {
    parent: apimApis[i]
    name: 'policy'
    properties: {
      format: 'xml'
      value: '<policies><inbound><base /><set-backend-service backend-id="func-${prefix}-${svc.key}-${environmentName}" /></inbound><backend><base /></backend><outbound><base /></outbound><on-error><base /></on-error></policies>'
    }
    dependsOn: [apimBackends[i]]
  }
]

// ─── Outputs ────────────────────────────────────────────────────────────────────

output storageAccountId string = storageAccount.id
output cosmosAccountEndpoint string = cosmosAccount.properties.documentEndpoint
output functionAppNames string[] = [for (svc, i) in microservices: funcApps[i].name]
output functionAppHostNames string[] = [
  for (svc, i) in microservices: funcApps[i].properties.defaultHostName
]
output apimGatewayUrl string = apim.properties.gatewayUrl
output staticWebAppDefaultHostname string = staticWebApp.properties.defaultHostname
