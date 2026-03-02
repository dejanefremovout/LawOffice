@secure()
param operations_delete_deletecase_type string

@secure()
param operations_delete_deletecase_type_1 string

@secure()
param operations_delete_deletedocumentfile_type string

@secure()
param operations_delete_deletedocumentfile_type_1 string

@secure()
param operations_delete_deletehearing_type string

@secure()
param operations_delete_deletehearing_type_1 string

@secure()
param operations_get_getallcases_type string

@secure()
param operations_get_getalldocumentfiles_type string

@secure()
param operations_get_getalldocumentfiles_type_1 string

@secure()
param operations_get_getallhearings_type string

@secure()
param operations_get_getallhearings_type_1 string

@secure()
param operations_get_getcase_type string

@secure()
param operations_get_getcase_type_1 string

@secure()
param operations_get_getdocumentfile_type string

@secure()
param operations_get_getdocumentfile_type_1 string

@secure()
param operations_get_gethearing_type string

@secure()
param operations_get_gethearing_type_1 string

@secure()
param operations_get_getalllawyers_type string

@secure()
param operations_get_getlawyer_type string

@secure()
param operations_get_getlawyer_type_1 string

@secure()
param operations_get_getoffice_type string

@secure()
param operations_get_getallclients_type string

@secure()
param operations_get_getallopposingparties_type string

@secure()
param operations_get_getclient_type string

@secure()
param operations_get_getclient_type_1 string

@secure()
param operations_get_getopposingparty_type string

@secure()
param operations_get_getopposingparty_type_1 string

@secure()
param properties_func_lawoffice_casemanagement_dev_key_value string

@secure()
param properties_func_lawoffice_officemanagement_dev_key_value string

@secure()
param properties_func_lawoffice_partymanagement_dev_key_value string
param staticSites_swaf_lawoffice_dev_name string
param storageAccounts_rglawofficedev_name string
param serverfarms_ASP_rglawofficedev_86bd_name string
param serverfarms_ASP_rglawofficedev_a039_name string
param serverfarms_ASP_rglawofficedev_a643_name string
param service_apim_lawoffice_dev_name string
param sites_func_lawoffice_casemanagement_dev_name string
param sites_func_lawoffice_partymanagement_dev_name string
param sites_func_lawoffice_officemanagement_dev_name string
param databaseAccounts_cos_lawoffice_officemanagement_dev_name string

resource service_apim_lawoffice_dev_name_resource 'Microsoft.ApiManagement/service@2024-06-01-preview' = {
  name: service_apim_lawoffice_dev_name
  location: 'West Europe'
  sku: {
    name: 'Consumption'
    capacity: 0
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    publisherEmail: 'dejan.efremov@outlook.com'
    publisherName: 'LawOffice'
    notificationSenderEmail: 'apimgmt-noreply@mail.windowsazure.com'
    hostnameConfigurations: [
      {
        type: 'Proxy'
        hostName: '${service_apim_lawoffice_dev_name}.azure-api.net'
        negotiateClientCertificate: false
        defaultSslBinding: true
        certificateSource: 'BuiltIn'
      }
    ]
    customProperties: {
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls10': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Protocols.Tls11': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls10': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Tls11': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Security.Backend.Protocols.Ssl30': 'False'
      'Microsoft.WindowsAzure.ApiManagement.Gateway.Protocols.Server.Http2': 'False'
    }
    virtualNetworkType: 'None'
    disableGateway: false
    natGatewayState: 'Unsupported'
    apiVersionConstraint: {}
    publicNetworkAccess: 'Enabled'
    legacyPortalStatus: 'Disabled'
    developerPortalStatus: 'Enabled'
    releaseChannel: 'Default'
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource 'Microsoft.DocumentDB/databaseAccounts@2025-05-01-preview' = {
  name: databaseAccounts_cos_lawoffice_officemanagement_dev_name
  location: 'West Europe'
  tags: {
    defaultExperience: 'Core (SQL)'
    'hidden-workload-type': 'Learning'
    'hidden-cosmos-mmspecial': ''
  }
  kind: 'GlobalDocumentDB'
  identity: {
    type: 'None'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    enableAutomaticFailover: true
    enableMultipleWriteLocations: false
    isVirtualNetworkFilterEnabled: false
    virtualNetworkRules: []
    disableKeyBasedMetadataWriteAccess: false
    enableFreeTier: false
    enableAnalyticalStorage: false
    analyticalStorageConfiguration: {
      schemaType: 'WellDefined'
    }
    databaseAccountOfferType: 'Standard'
    enableMaterializedViews: false
    capacityMode: 'Serverless'
    defaultIdentity: 'FirstPartyIdentity'
    networkAclBypass: 'None'
    disableLocalAuth: false
    enablePartitionMerge: false
    enablePerRegionPerPartitionAutoscale: false
    enableBurstCapacity: false
    enablePriorityBasedExecution: false
    defaultPriorityLevel: 'High'
    minimalTlsVersion: 'Tls12'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    locations: [
      {
        locationName: 'West Europe'
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    cors: []
    capabilities: []
    ipRules: []
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 8
        backupStorageRedundancy: 'Local'
      }
    }
    networkAclBypassResourceIds: []
    diagnosticLogSettings: {
      enableFullTextQuery: 'None'
    }
    capacity: {
      totalThroughputLimit: 4000
    }
  }
}

resource storageAccounts_rglawofficedev_name_resource 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: storageAccounts_rglawofficedev_name
  location: 'westeurope'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
  kind: 'StorageV2'
  properties: {
    dnsEndpointType: 'Standard'
    defaultToOAuthAuthentication: false
    publicNetworkAccess: 'Enabled'
    allowCrossTenantReplication: false
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      requireInfrastructureEncryption: false
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}

resource serverfarms_ASP_rglawofficedev_86bd_name_resource 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: serverfarms_ASP_rglawofficedev_86bd_name
  location: 'West Europe'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  kind: 'functionapp'
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: false
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
    asyncScalingEnabled: false
  }
}

resource serverfarms_ASP_rglawofficedev_a039_name_resource 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: serverfarms_ASP_rglawofficedev_a039_name
  location: 'West Europe'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  kind: 'functionapp'
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: false
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
    asyncScalingEnabled: false
  }
}

resource serverfarms_ASP_rglawofficedev_a643_name_resource 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: serverfarms_ASP_rglawofficedev_a643_name
  location: 'West Europe'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  kind: 'functionapp'
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: false
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
    asyncScalingEnabled: false
  }
}

resource staticSites_swaf_lawoffice_dev_name_resource 'Microsoft.Web/staticSites@2024-11-01' = {
  name: staticSites_swaf_lawoffice_dev_name
  location: 'West Europe'
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    repositoryUrl: 'https://github.com/dejanefremovout/LawOffice'
    branch: 'master'
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    provider: 'GitHub'
    enterpriseGradeCdnStatus: 'Disabled'
  }
}

resource service_apim_lawoffice_dev_name_CaseManagement 'Microsoft.ApiManagement/service/apis@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'CaseManagement'
  properties: {
    displayName: 'CaseManagement'
    apiRevision: '1'
    description: 'Import from "func-lawoffice-casemanagement-dev" Function App'
    subscriptionRequired: false
    path: 'case'
    protocols: [
      'https'
    ]
    authenticationSettings: {
      oAuth2AuthenticationSettings: []
      openidAuthenticationSettings: []
    }
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    isCurrent: true
  }
}

resource service_apim_lawoffice_dev_name_OfficeManagement 'Microsoft.ApiManagement/service/apis@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'OfficeManagement'
  properties: {
    displayName: 'OfficeManagement'
    apiRevision: '1'
    description: 'Import from "func-lawoffice-officemanagement-dev" Function App'
    subscriptionRequired: false
    path: 'office'
    protocols: [
      'https'
    ]
    authenticationSettings: {
      oAuth2AuthenticationSettings: []
      openidAuthenticationSettings: []
    }
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    isCurrent: true
  }
}

resource service_apim_lawoffice_dev_name_PartyManagement 'Microsoft.ApiManagement/service/apis@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'PartyManagement'
  properties: {
    displayName: 'PartyManagement'
    apiRevision: '1'
    description: 'Import from "func-lawoffice-partymanagement-dev" Function App'
    subscriptionRequired: false
    path: 'party'
    protocols: [
      'https'
    ]
    authenticationSettings: {
      oAuth2AuthenticationSettings: []
      openidAuthenticationSettings: []
    }
    subscriptionKeyParameterNames: {
      header: 'Ocp-Apim-Subscription-Key'
      query: 'subscription-key'
    }
    isCurrent: true
  }
}

resource service_apim_lawoffice_dev_name_func_lawoffice_casemanagement_dev 'Microsoft.ApiManagement/service/backends@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-casemanagement-dev'
  properties: {
    description: 'func-lawoffice-casemanagement-dev'
    url: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api'
    protocol: 'http'
    resourceId: 'https://management.azure.com/subscriptions/6578ff96-79dc-4c29-810c-0966bc125a06/resourceGroups/rg-lawoffice-dev/providers/Microsoft.Web/sites/func-lawoffice-casemanagement-dev'
    credentials: {
      header: {
        'x-functions-key': [
          '{{func-lawoffice-casemanagement-dev-key}}'
        ]
      }
    }
  }
}

resource service_apim_lawoffice_dev_name_func_lawoffice_officemanagement_dev 'Microsoft.ApiManagement/service/backends@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-officemanagement-dev'
  properties: {
    description: 'func-lawoffice-officemanagement-dev'
    url: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api'
    protocol: 'http'
    resourceId: 'https://management.azure.com/subscriptions/6578ff96-79dc-4c29-810c-0966bc125a06/resourceGroups/rg-lawoffice-dev/providers/Microsoft.Web/sites/func-lawoffice-officemanagement-dev'
    credentials: {
      header: {
        'x-functions-key': [
          '{{func-lawoffice-officemanagement-dev-key}}'
        ]
      }
    }
  }
}

resource service_apim_lawoffice_dev_name_func_lawoffice_partymanagement_dev 'Microsoft.ApiManagement/service/backends@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-partymanagement-dev'
  properties: {
    description: 'func-lawoffice-partymanagement-dev'
    url: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api'
    protocol: 'http'
    resourceId: 'https://management.azure.com/subscriptions/6578ff96-79dc-4c29-810c-0966bc125a06/resourceGroups/rg-lawoffice-dev/providers/Microsoft.Web/sites/func-lawoffice-partymanagement-dev'
    credentials: {
      header: {
        'x-functions-key': [
          '{{func-lawoffice-partymanagement-dev-key}}'
        ]
      }
    }
  }
}

resource service_apim_lawoffice_dev_name_func_lawoffice_casemanagement_dev_key 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-casemanagement-dev-key'
  properties: {
    displayName: 'func-lawoffice-casemanagement-dev-key'
    tags: [
      'key'
      'function'
      'auto'
    ]
    secret: true
  }
}

resource service_apim_lawoffice_dev_name_func_lawoffice_officemanagement_dev_key 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-officemanagement-dev-key'
  properties: {
    displayName: 'func-lawoffice-officemanagement-dev-key'
    tags: [
      'key'
      'function'
      'auto'
    ]
    secret: true
  }
}

resource service_apim_lawoffice_dev_name_func_lawoffice_partymanagement_dev_key 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-partymanagement-dev-key'
  properties: {
    displayName: 'func-lawoffice-partymanagement-dev-key'
    tags: [
      'key'
      'function'
      'auto'
    ]
    secret: true
  }
}

resource service_apim_lawoffice_dev_name_policy 'Microsoft.ApiManagement/service/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'policy'
  properties: {
    value: '<!--\r\n    IMPORTANT:\r\n    - Policy elements can appear only within the <inbound>, <outbound>, <backend> section elements.\r\n    - Only the <forward-request> policy element can appear within the <backend> section element.\r\n    - To apply a policy to the incoming request (before it is forwarded to the backend service), place a corresponding policy element within the <inbound> section element.\r\n    - To apply a policy to the outgoing response (before it is sent back to the caller), place a corresponding policy element within the <outbound> section element.\r\n    - To add a policy position the cursor at the desired insertion point and click on the round button associated with the policy.\r\n    - To remove a policy, delete the corresponding policy statement from the policy document.\r\n    - Policies are applied in the order of their appearance, from the top down.\r\n-->\r\n<policies>\r\n  <inbound>\r\n    <cors allow-credentials="true">\r\n      <allowed-origins>\r\n        <origin>https://green-sea-058b76203.4.azurestaticapps.net</origin>\r\n        <origin>http://localhost:4200</origin>\r\n      </allowed-origins>\r\n      <allowed-methods preflight-result-max-age="300">\r\n        <method>GET</method>\r\n        <method>POST</method>\r\n        <method>PUT</method>\r\n        <method>PATCH</method>\r\n        <method>DELETE</method>\r\n      </allowed-methods>\r\n      <allowed-headers>\r\n        <header>*</header>\r\n      </allowed-headers>\r\n      <expose-headers>\r\n        <header>*</header>\r\n      </expose-headers>\r\n    </cors>\r\n    <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized. Access token is missing or invalid.">\r\n      <openid-config url="https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0/.well-known/openid-configuration" />\r\n      <audiences>\r\n        <audience>a9a5990c-f11e-49df-a582-a2c1416456cf</audience>\r\n      </audiences>\r\n      <issuers>\r\n        <issuer>https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0</issuer>\r\n      </issuers>\r\n    </validate-jwt>\r\n  </inbound>\r\n  <backend>\r\n    <forward-request />\r\n  </backend>\r\n  <outbound />\r\n  <on-error />\r\n</policies>'
    format: 'xml'
  }
}

resource Microsoft_ApiManagement_service_properties_service_apim_lawoffice_dev_name_func_lawoffice_casemanagement_dev_key 'Microsoft.ApiManagement/service/properties@2019-01-01' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-casemanagement-dev-key'
  properties: {
    displayName: 'func-lawoffice-casemanagement-dev-key'
    tags: [
      'key'
      'function'
      'auto'
    ]
    secret: true
    value: properties_func_lawoffice_casemanagement_dev_key_value
  }
}

resource Microsoft_ApiManagement_service_properties_service_apim_lawoffice_dev_name_func_lawoffice_officemanagement_dev_key 'Microsoft.ApiManagement/service/properties@2019-01-01' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-officemanagement-dev-key'
  properties: {
    displayName: 'func-lawoffice-officemanagement-dev-key'
    tags: [
      'key'
      'function'
      'auto'
    ]
    secret: true
    value: properties_func_lawoffice_officemanagement_dev_key_value
  }
}

resource Microsoft_ApiManagement_service_properties_service_apim_lawoffice_dev_name_func_lawoffice_partymanagement_dev_key 'Microsoft.ApiManagement/service/properties@2019-01-01' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'func-lawoffice-partymanagement-dev-key'
  properties: {
    displayName: 'func-lawoffice-partymanagement-dev-key'
    tags: [
      'key'
      'function'
      'auto'
    ]
    secret: true
    value: properties_func_lawoffice_partymanagement_dev_key_value
  }
}

resource service_apim_lawoffice_dev_name_master 'Microsoft.ApiManagement/service/subscriptions@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_resource
  name: 'master'
  properties: {
    scope: '${service_apim_lawoffice_dev_name_resource.id}/'
    displayName: 'Built-in all-access subscription'
    state: 'active'
    allowTracing: false
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000003 'Microsoft.DocumentDB/databaseAccounts/cassandraRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000003'
  properties: {
    roleName: 'Cosmos DB Cassandra Built-in Data Reader'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000004 'Microsoft.DocumentDB/databaseAccounts/cassandraRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000004'
  properties: {
    roleName: 'Cosmos DB Cassandra Built-in Data Contributor'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/write'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/*'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/write'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/delete'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource Microsoft_DocumentDB_databaseAccounts_gremlinRoleDefinitions_databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000003 'Microsoft.DocumentDB/databaseAccounts/gremlinRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000003'
  properties: {
    roleName: 'Cosmos DB Gremlin Built-in Data Reader'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
  }
}

resource Microsoft_DocumentDB_databaseAccounts_gremlinRoleDefinitions_databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000004 'Microsoft.DocumentDB/databaseAccounts/gremlinRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000004'
  properties: {
    roleName: 'Cosmos DB Gremlin Built-in Data Contributor'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/write'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/*'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/write'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/delete'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource Microsoft_DocumentDB_databaseAccounts_mongoMIRoleDefinitions_databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000003 'Microsoft.DocumentDB/databaseAccounts/mongoMIRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000003'
  properties: {
    roleName: 'Cosmos DB Mongo Built-in Data Reader'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
  }
}

resource Microsoft_DocumentDB_databaseAccounts_mongoMIRoleDefinitions_databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000004 'Microsoft.DocumentDB/databaseAccounts/mongoMIRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000004'
  properties: {
    roleName: 'Cosmos DB Mongo Built-in Data Contributor'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/write'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/*'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/write'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/delete'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: 'casemanagement'
  properties: {
    resource: {
      id: 'casemanagement'
    }
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_officemanagement 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: 'officemanagement'
  properties: {
    resource: {
      id: 'officemanagement'
    }
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_partymanagement 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: 'partymanagement'
  properties: {
    resource: {
      id: 'partymanagement'
    }
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000001 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000001'
  properties: {
    roleName: 'Cosmos DB Built-in Data Reader'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/read'
        ]
        notDataActions: []
      }
    ]
  }
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000002 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000002'
  properties: {
    roleName: 'Cosmos DB Built-in Data Contributor'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource Microsoft_DocumentDB_databaseAccounts_tableRoleDefinitions_databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000001 'Microsoft.DocumentDB/databaseAccounts/tableRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000001'
  properties: {
    roleName: 'Cosmos DB Built-in Data Reader'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
  }
}

resource Microsoft_DocumentDB_databaseAccounts_tableRoleDefinitions_databaseAccounts_cos_lawoffice_officemanagement_dev_name_00000000_0000_0000_0000_000000000002 'Microsoft.DocumentDB/databaseAccounts/tableRoleDefinitions@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  name: '00000000-0000-0000-0000-000000000002'
  properties: {
    roleName: 'Cosmos DB Built-in Data Contributor'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/tables/*'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource storageAccounts_rglawofficedev_name_default 'Microsoft.Storage/storageAccounts/blobServices@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_resource
  name: 'default'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
  properties: {
    containerDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
    cors: {
      corsRules: [
        {
          allowedOrigins: [
            'https://green-sea-058b76203.4.azurestaticapps.net'
          ]
          allowedMethods: [
            'GET'
            'PUT'
            'OPTIONS'
          ]
          maxAgeInSeconds: 3600
          exposedHeaders: [
            'x-ms-request-id'
            'x-ms-version'
          ]
          allowedHeaders: [
            'x-ms-blob-type'
            'content-type'
          ]
        }
      ]
    }
    deleteRetentionPolicy: {
      allowPermanentDelete: false
      enabled: true
      days: 7
    }
  }
}

resource Microsoft_Storage_storageAccounts_fileServices_storageAccounts_rglawofficedev_name_default 'Microsoft.Storage/storageAccounts/fileServices@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_resource
  name: 'default'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
  properties: {
    protocolSettings: {
      smb: {}
    }
    cors: {
      corsRules: []
    }
    shareDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

resource Microsoft_Storage_storageAccounts_queueServices_storageAccounts_rglawofficedev_name_default 'Microsoft.Storage/storageAccounts/queueServices@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource Microsoft_Storage_storageAccounts_tableServices_storageAccounts_rglawofficedev_name_default 'Microsoft.Storage/storageAccounts/tableServices@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_resource 'Microsoft.Web/sites@2024-11-01' = {
  name: sites_func_lawoffice_casemanagement_dev_name
  location: 'West Europe'
  kind: 'functionapp'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${sites_func_lawoffice_casemanagement_dev_name}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${sites_func_lawoffice_casemanagement_dev_name}.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: serverfarms_ASP_rglawofficedev_a643_name_resource.id
    reserved: false
    isXenon: false
    hyperV: false
    dnsConfiguration: {}
    outboundVnetRouting: {
      allTraffic: false
      applicationTraffic: false
      contentShareTraffic: false
      imagePullTraffic: false
      backupRestoreTraffic: false
    }
    siteConfig: {
      numberOfWorkers: 1
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: false
      functionAppScaleLimit: 200
      minimumElasticInstanceCount: 0
    }
    scmSiteAlsoStopped: false
    clientAffinityEnabled: false
    clientAffinityProxyEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    ipMode: 'IPv4'
    customDomainVerificationId: '51A347847F442EEB8CA19CE0A73615CC7649D7DB8D7318DE1ECFD16FD9A1DA51'
    containerSize: 1536
    dailyMemoryTimeQuota: 0
    httpsOnly: true
    endToEndEncryptionEnabled: false
    redundancyMode: 'None'
    publicNetworkAccess: 'Enabled'
    storageAccountRequired: false
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_resource 'Microsoft.Web/sites@2024-11-01' = {
  name: sites_func_lawoffice_officemanagement_dev_name
  location: 'West Europe'
  kind: 'functionapp'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${sites_func_lawoffice_officemanagement_dev_name}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${sites_func_lawoffice_officemanagement_dev_name}.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: serverfarms_ASP_rglawofficedev_86bd_name_resource.id
    reserved: false
    isXenon: false
    hyperV: false
    dnsConfiguration: {}
    outboundVnetRouting: {
      allTraffic: false
      applicationTraffic: false
      contentShareTraffic: false
      imagePullTraffic: false
      backupRestoreTraffic: false
    }
    siteConfig: {
      numberOfWorkers: 1
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: false
      functionAppScaleLimit: 200
      minimumElasticInstanceCount: 0
    }
    scmSiteAlsoStopped: false
    clientAffinityEnabled: false
    clientAffinityProxyEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    ipMode: 'IPv4'
    customDomainVerificationId: '51A347847F442EEB8CA19CE0A73615CC7649D7DB8D7318DE1ECFD16FD9A1DA51'
    containerSize: 1536
    dailyMemoryTimeQuota: 0
    httpsOnly: true
    endToEndEncryptionEnabled: false
    redundancyMode: 'None'
    publicNetworkAccess: 'Enabled'
    storageAccountRequired: false
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_resource 'Microsoft.Web/sites@2024-11-01' = {
  name: sites_func_lawoffice_partymanagement_dev_name
  location: 'West Europe'
  kind: 'functionapp'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${sites_func_lawoffice_partymanagement_dev_name}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${sites_func_lawoffice_partymanagement_dev_name}.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: serverfarms_ASP_rglawofficedev_a039_name_resource.id
    reserved: false
    isXenon: false
    hyperV: false
    dnsConfiguration: {}
    outboundVnetRouting: {
      allTraffic: false
      applicationTraffic: false
      contentShareTraffic: false
      imagePullTraffic: false
      backupRestoreTraffic: false
    }
    siteConfig: {
      numberOfWorkers: 1
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: false
      functionAppScaleLimit: 200
      minimumElasticInstanceCount: 0
    }
    scmSiteAlsoStopped: false
    clientAffinityEnabled: false
    clientAffinityProxyEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    ipMode: 'IPv4'
    customDomainVerificationId: '51A347847F442EEB8CA19CE0A73615CC7649D7DB8D7318DE1ECFD16FD9A1DA51'
    containerSize: 1536
    dailyMemoryTimeQuota: 0
    httpsOnly: true
    endToEndEncryptionEnabled: false
    redundancyMode: 'None'
    publicNetworkAccess: 'Enabled'
    storageAccountRequired: false
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_ftp 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'ftp'
  location: 'West Europe'
  properties: {
    allow: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_ftp 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'ftp'
  location: 'West Europe'
  properties: {
    allow: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_ftp 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'ftp'
  location: 'West Europe'
  properties: {
    allow: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'scm'
  location: 'West Europe'
  properties: {
    allow: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'scm'
  location: 'West Europe'
  properties: {
    allow: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'scm'
  location: 'West Europe'
  properties: {
    allow: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_web 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'web'
  location: 'West Europe'
  properties: {
    numberOfWorkers: 1
    defaultDocuments: [
      'Default.htm'
      'Default.html'
      'Default.asp'
      'index.htm'
      'index.html'
      'iisstart.htm'
      'default.aspx'
      'index.php'
    ]
    netFrameworkVersion: 'v10.0'
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    httpLoggingEnabled: false
    acrUseManagedIdentityCreds: false
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: false
    publishingUsername: 'REDACTED'
    scmType: 'GitHubAction'
    use32BitWorkerProcess: false
    webSocketsEnabled: false
    alwaysOn: false
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    loadBalancing: 'LeastRequests'
    experiments: {
      rampUpRules: []
    }
    autoHealEnabled: false
    vnetRouteAllEnabled: false
    vnetPrivatePortsCount: 0
    publicNetworkAccess: 'Enabled'
    cors: {
      allowedOrigins: [
        'https://portal.azure.com'
      ]
      supportCredentials: false
    }
    localMySqlEnabled: false
    ipSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    http20Enabled: false
    minTlsVersion: '1.2'
    scmMinTlsVersion: '1.2'
    ftpsState: 'FtpsOnly'
    preWarmedInstanceCount: 0
    functionAppScaleLimit: 200
    functionsRuntimeScaleMonitoringEnabled: false
    minimumElasticInstanceCount: 0
    azureStorageAccounts: {}
    http20ProxyFlag: 0
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_web 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'web'
  location: 'West Europe'
  properties: {
    numberOfWorkers: 1
    defaultDocuments: [
      'Default.htm'
      'Default.html'
      'Default.asp'
      'index.htm'
      'index.html'
      'iisstart.htm'
      'default.aspx'
      'index.php'
    ]
    netFrameworkVersion: 'v10.0'
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    httpLoggingEnabled: false
    acrUseManagedIdentityCreds: false
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: false
    publishingUsername: 'REDACTED'
    scmType: 'GitHubAction'
    use32BitWorkerProcess: false
    webSocketsEnabled: false
    alwaysOn: false
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    loadBalancing: 'LeastRequests'
    experiments: {
      rampUpRules: []
    }
    autoHealEnabled: false
    vnetRouteAllEnabled: false
    vnetPrivatePortsCount: 0
    publicNetworkAccess: 'Enabled'
    cors: {
      allowedOrigins: [
        'https://portal.azure.com'
      ]
      supportCredentials: false
    }
    localMySqlEnabled: false
    ipSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    http20Enabled: false
    minTlsVersion: '1.2'
    scmMinTlsVersion: '1.2'
    ftpsState: 'FtpsOnly'
    preWarmedInstanceCount: 0
    functionAppScaleLimit: 200
    functionsRuntimeScaleMonitoringEnabled: false
    minimumElasticInstanceCount: 0
    azureStorageAccounts: {}
    http20ProxyFlag: 0
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_web 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'web'
  location: 'West Europe'
  properties: {
    numberOfWorkers: 1
    defaultDocuments: [
      'Default.htm'
      'Default.html'
      'Default.asp'
      'index.htm'
      'index.html'
      'iisstart.htm'
      'default.aspx'
      'index.php'
    ]
    netFrameworkVersion: 'v10.0'
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    httpLoggingEnabled: false
    acrUseManagedIdentityCreds: false
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: false
    publishingUsername: 'REDACTED'
    scmType: 'GitHubAction'
    use32BitWorkerProcess: false
    webSocketsEnabled: false
    alwaysOn: false
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    loadBalancing: 'LeastRequests'
    experiments: {
      rampUpRules: []
    }
    autoHealEnabled: false
    vnetRouteAllEnabled: false
    vnetPrivatePortsCount: 0
    publicNetworkAccess: 'Enabled'
    cors: {
      allowedOrigins: [
        'https://portal.azure.com'
      ]
      supportCredentials: false
    }
    localMySqlEnabled: false
    ipSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 2147483647
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    http20Enabled: false
    minTlsVersion: '1.2'
    scmMinTlsVersion: '1.2'
    ftpsState: 'FtpsOnly'
    preWarmedInstanceCount: 0
    functionAppScaleLimit: 200
    functionsRuntimeScaleMonitoringEnabled: false
    minimumElasticInstanceCount: 0
    azureStorageAccounts: {}
    http20ProxyFlag: 0
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_1dcd477e421f41819e1d8daa4ac7d269 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: '1dcd477e421f41819e1d8daa4ac7d269'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"c16d6d8061bd3b3bf26669593c50de2fa449ca8b","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-24T10:50:12.7453329Z'
    end_time: '2026-02-24T10:50:14.4484762Z'
    active: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_25aa5a498aee4588b03a57707cf6f978 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: '25aa5a498aee4588b03a57707cf6f978'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"219d0c94af0b798b6c0297855129787102769e2e","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-27T13:35:56.4260766Z'
    end_time: '2026-02-27T13:35:58.0198291Z'
    active: true
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_293e8a2b51c34de3932c1faff46e5396 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: '293e8a2b51c34de3932c1faff46e5396'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"e7cda165d72e5acb977f5019dd7c89bed47f4a4f","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-23T20:26:27.2217149Z'
    end_time: '2026-02-23T20:26:28.7069504Z'
    active: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_5eaaa320c08a4cc9a4768e6799e1617f 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: '5eaaa320c08a4cc9a4768e6799e1617f'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"c16d6d8061bd3b3bf26669593c50de2fa449ca8b","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-24T10:50:42.8010736Z'
    end_time: '2026-02-24T10:50:44.5670148Z'
    active: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_6b817683df1848f79d1e52943256cd66 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: '6b817683df1848f79d1e52943256cd66'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"c16d6d8061bd3b3bf26669593c50de2fa449ca8b","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-24T10:51:47.0411913Z'
    end_time: '2026-02-24T10:51:48.7911999Z'
    active: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_734caa18ec7a40619077787dd59252d5 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: '734caa18ec7a40619077787dd59252d5'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"a5cb117751baf20a4051c814f79a230ebc57c613","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-23T20:24:45.3891178Z'
    end_time: '2026-02-23T20:24:47.2172398Z'
    active: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_77ea9a57ad204deab0d4743664b0e132 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: '77ea9a57ad204deab0d4743664b0e132'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"bcac249a7986a32467254594d1087db7c63329fc","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-24T09:27:50.318273Z'
    end_time: '2026-02-24T09:27:51.7870238Z'
    active: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_7ff0e99ed70e47aea334c322168f361c 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: '7ff0e99ed70e47aea334c322168f361c'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"219d0c94af0b798b6c0297855129787102769e2e","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-27T13:35:17.9532758Z'
    end_time: '2026-02-27T13:35:19.5323202Z'
    active: true
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_9e982bee61a841f581c9b3f1c8e046cc 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: '9e982bee61a841f581c9b3f1c8e046cc'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"43cf3cf5296abe674fb84163088d132acf0e5263","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-23T20:43:21.0479483Z'
    end_time: '2026-02-23T20:43:22.5635603Z'
    active: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_e20aee851c38416b916dd64731e17e6f 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'e20aee851c38416b916dd64731e17e6f'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"a016c7e2afc80a041d4a2347de2b98ccac94c160","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-27T13:24:38.6188914Z'
    end_time: '2026-02-27T13:24:40.8845585Z'
    active: true
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_e52499d977ac4bb7bd11598e28354d78 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'e52499d977ac4bb7bd11598e28354d78'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"7bc4872d547b9f657a43d59ba51298cd8037076d","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-24T14:29:25.177208Z'
    end_time: '2026-02-24T14:29:26.5835468Z'
    active: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_e85c891f8e21446b941ea8b96663d34d 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'e85c891f8e21446b941ea8b96663d34d'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"15416ef23c19978c34c1c4aceacecb2f046506ea","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-24T09:41:08.882096Z'
    end_time: '2026-02-24T09:41:10.5383448Z'
    active: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_e99e3d2594924da98c248e094ded7ef4 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'e99e3d2594924da98c248e094ded7ef4'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"e7cda165d72e5acb977f5019dd7c89bed47f4a4f","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-23T20:26:00.9332895Z'
    end_time: '2026-02-23T20:26:02.8707759Z'
    active: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_ef7ced3f24c642da9972986f54ed4d82 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'ef7ced3f24c642da9972986f54ed4d82'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"43cf3cf5296abe674fb84163088d132acf0e5263","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-23T20:42:53.1012749Z'
    end_time: '2026-02-23T20:42:54.6794041Z'
    active: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_f05eb8ec8b06458fad4cf85dfc7b234b 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'f05eb8ec8b06458fad4cf85dfc7b234b'
  location: 'West Europe'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY_FUNCTIONS_V1'
    message: '{"type":"deployment","sha":"8bf65831b4598ac220f33eb22ec8d9c42d18ba76","repoName":"dejanefremovout/LawOffice","actor":"dejanefremovout","slotName":"production"}'
    start_time: '2026-02-23T16:37:05.6624962Z'
    end_time: '2026-02-23T16:37:07.3187518Z'
    active: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_DeleteCase 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'DeleteCase'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/DeleteCase.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/DeleteCase'
    config: {
      name: 'DeleteCase'
      entryPoint: 'CaseManagement.Api.Functions.CaseFunction.Delete'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'delete'
          ]
          route: 'case/{officeId}/{caseId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/case/{officeid}/{caseid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_DeleteDocumentFile 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'DeleteDocumentFile'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/DeleteDocumentFile.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/DeleteDocumentFile'
    config: {
      name: 'DeleteDocumentFile'
      entryPoint: 'CaseManagement.Api.Functions.DocumentFileFunction.Delete'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'delete'
          ]
          route: 'documentFile/{officeId}/{documentFileId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/documentfile/{officeid}/{documentfileid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_DeleteHearing 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'DeleteHearing'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/DeleteHearing.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/DeleteHearing'
    config: {
      name: 'DeleteHearing'
      entryPoint: 'CaseManagement.Api.Functions.HearingFunction.Delete'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'delete'
          ]
          route: 'hearing/{officeId}/{hearingId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/hearing/{officeid}/{hearingid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_GetAllCases 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'GetAllCases'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetAllCases.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/GetAllCases'
    config: {
      name: 'GetAllCases'
      entryPoint: 'CaseManagement.Api.Functions.CaseFunction.GetAll'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'case/{officeId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/case/{officeid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_GetAllClients 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'GetAllClients'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetAllClients.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/GetAllClients'
    config: {
      name: 'GetAllClients'
      entryPoint: 'PartyManagement.Api.Functions.ClientFunction.GetAll'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'client/{officeId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/client/{officeid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_GetAllDocumentFiles 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'GetAllDocumentFiles'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetAllDocumentFiles.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/GetAllDocumentFiles'
    config: {
      name: 'GetAllDocumentFiles'
      entryPoint: 'CaseManagement.Api.Functions.DocumentFileFunction.GetAll'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'documentFile/{officeId}/case/{caseId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/documentfile/{officeid}/case/{caseid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_GetAllHearings 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'GetAllHearings'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetAllHearings.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/GetAllHearings'
    config: {
      name: 'GetAllHearings'
      entryPoint: 'CaseManagement.Api.Functions.HearingFunction.GetAll'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'hearing/{officeId}/case/{caseId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/hearing/{officeid}/case/{caseid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_GetAllLawyers 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'GetAllLawyers'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetAllLawyers.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/GetAllLawyers'
    config: {
      name: 'GetAllLawyers'
      entryPoint: 'OfficeManagement.Api.Functions.LawyerFunction.GetAll'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'lawyer/{officeId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/lawyer/{officeid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_GetAllOpposingParties 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'GetAllOpposingParties'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetAllOpposingParties.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/GetAllOpposingParties'
    config: {
      name: 'GetAllOpposingParties'
      entryPoint: 'PartyManagement.Api.Functions.OpposingPartyFunction.GetAll'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'opposingParty/{officeId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/opposingparty/{officeid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_GetCase 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'GetCase'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetCase.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/GetCase'
    config: {
      name: 'GetCase'
      entryPoint: 'CaseManagement.Api.Functions.CaseFunction.Get'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'case/{officeId}/{caseId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/case/{officeid}/{caseid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_GetClient 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'GetClient'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetClient.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/GetClient'
    config: {
      name: 'GetClient'
      entryPoint: 'PartyManagement.Api.Functions.ClientFunction.Get'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'client/{officeId}/{clientId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/client/{officeid}/{clientid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_GetDocumentFile 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'GetDocumentFile'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetDocumentFile.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/GetDocumentFile'
    config: {
      name: 'GetDocumentFile'
      entryPoint: 'CaseManagement.Api.Functions.DocumentFileFunction.Get'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'documentFile/{officeId}/{documentFileId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/documentfile/{officeid}/{documentfileid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_GetHearing 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'GetHearing'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetHearing.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/GetHearing'
    config: {
      name: 'GetHearing'
      entryPoint: 'CaseManagement.Api.Functions.HearingFunction.Get'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'hearing/{officeId}/{hearingId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/hearing/{officeid}/{hearingid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_GetLawyer 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'GetLawyer'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetLawyer.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/GetLawyer'
    config: {
      name: 'GetLawyer'
      entryPoint: 'OfficeManagement.Api.Functions.LawyerFunction.Get'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'lawyer/{officeId}/{lawyerId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/lawyer/{officeid}/{lawyerid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_GetOffice 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'GetOffice'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetOffice.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/GetOffice'
    config: {
      name: 'GetOffice'
      entryPoint: 'OfficeManagement.Api.Functions.OfficeFunction.Get'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'office/{officeId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/office/{officeid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_GetOpposingParty 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'GetOpposingParty'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/GetOpposingParty.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/GetOpposingParty'
    config: {
      name: 'GetOpposingParty'
      entryPoint: 'PartyManagement.Api.Functions.OpposingPartyFunction.Get'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'get'
          ]
          route: 'opposingParty/{officeId}/{opposingPartyId}'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/opposingparty/{officeid}/{opposingpartyid}'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_PostCase 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'PostCase'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostCase.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/PostCase'
    config: {
      name: 'PostCase'
      entryPoint: 'CaseManagement.Api.Functions.CaseFunction.Post'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'case'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/case'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_PostClient 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'PostClient'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostClient.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/PostClient'
    config: {
      name: 'PostClient'
      entryPoint: 'PartyManagement.Api.Functions.ClientFunction.Post'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'client'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/client'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_PostDocumentFile 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'PostDocumentFile'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostDocumentFile.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/PostDocumentFile'
    config: {
      name: 'PostDocumentFile'
      entryPoint: 'CaseManagement.Api.Functions.DocumentFileFunction.Post'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'documentFile'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/documentfile'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_PostHearing 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'PostHearing'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostHearing.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/PostHearing'
    config: {
      name: 'PostHearing'
      entryPoint: 'CaseManagement.Api.Functions.HearingFunction.Post'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'hearing'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/hearing'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_PostLawyer 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'PostLawyer'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostLawyer.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/PostLawyer'
    config: {
      name: 'PostLawyer'
      entryPoint: 'OfficeManagement.Api.Functions.LawyerFunction.Post'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'lawyer'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/lawyer'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_PostOffice 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'PostOffice'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostOffice.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/PostOffice'
    config: {
      name: 'PostOffice'
      entryPoint: 'OfficeManagement.Api.Functions.OfficeFunction.Post'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'office'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/office'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_PostOpposingParty 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'PostOpposingParty'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PostOpposingParty.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/PostOpposingParty'
    config: {
      name: 'PostOpposingParty'
      entryPoint: 'PartyManagement.Api.Functions.OpposingPartyFunction.Post'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'post'
          ]
          route: 'opposingParty'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/opposingparty'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_PutCase 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'PutCase'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutCase.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/PutCase'
    config: {
      name: 'PutCase'
      entryPoint: 'CaseManagement.Api.Functions.CaseFunction.Put'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'case'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/case'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_PutClient 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'PutClient'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutClient.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/PutClient'
    config: {
      name: 'PutClient'
      entryPoint: 'PartyManagement.Api.Functions.ClientFunction.Put'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'client'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/client'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_PutDocumentFile 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'PutDocumentFile'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutDocumentFile.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/PutDocumentFile'
    config: {
      name: 'PutDocumentFile'
      entryPoint: 'CaseManagement.Api.Functions.DocumentFileFunction.Put'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'documentFile'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/documentfile'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_PutHearing 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: 'PutHearing'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/CaseManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutHearing.dat'
    href: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/admin/functions/PutHearing'
    config: {
      name: 'PutHearing'
      entryPoint: 'CaseManagement.Api.Functions.HearingFunction.Put'
      scriptFile: 'CaseManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'hearing'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net/api/hearing'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_PutLawyer 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'PutLawyer'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutLawyer.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/PutLawyer'
    config: {
      name: 'PutLawyer'
      entryPoint: 'OfficeManagement.Api.Functions.LawyerFunction.Put'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'lawyer'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/lawyer'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_PutOffice 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: 'PutOffice'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/OfficeManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutOffice.dat'
    href: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/admin/functions/PutOffice'
    config: {
      name: 'PutOffice'
      entryPoint: 'OfficeManagement.Api.Functions.OfficeFunction.Put'
      scriptFile: 'OfficeManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'office'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net/api/office'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_PutOpposingParty 'Microsoft.Web/sites/functions@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: 'PutOpposingParty'
  location: 'West Europe'
  properties: {
    script_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/site/wwwroot/PartyManagement.Api.dll'
    test_data_href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/vfs/data/Functions/sampledata/PutOpposingParty.dat'
    href: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/admin/functions/PutOpposingParty'
    config: {
      name: 'PutOpposingParty'
      entryPoint: 'PartyManagement.Api.Functions.OpposingPartyFunction.Put'
      scriptFile: 'PartyManagement.Api.dll'
      language: 'dotnet-isolated'
      functionDirectory: ''
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'In'
          authLevel: 'Function'
          methods: [
            'put'
          ]
          route: 'opposingParty'
        }
        {
          name: '$return'
          type: 'http'
          direction: 'Out'
        }
      ]
    }
    invoke_url_template: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net/api/opposingparty'
    language: 'dotnet-isolated'
    isDisabled: false
  }
}

resource sites_func_lawoffice_casemanagement_dev_name_sites_func_lawoffice_casemanagement_dev_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2024-11-01' = {
  parent: sites_func_lawoffice_casemanagement_dev_name_resource
  name: '${sites_func_lawoffice_casemanagement_dev_name}.azurewebsites.net'
  location: 'West Europe'
  properties: {
    siteName: 'func-lawoffice-casemanagement-dev'
    hostNameType: 'Verified'
  }
}

resource sites_func_lawoffice_officemanagement_dev_name_sites_func_lawoffice_officemanagement_dev_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2024-11-01' = {
  parent: sites_func_lawoffice_officemanagement_dev_name_resource
  name: '${sites_func_lawoffice_officemanagement_dev_name}.azurewebsites.net'
  location: 'West Europe'
  properties: {
    siteName: 'func-lawoffice-officemanagement-dev'
    hostNameType: 'Verified'
  }
}

resource sites_func_lawoffice_partymanagement_dev_name_sites_func_lawoffice_partymanagement_dev_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2024-11-01' = {
  parent: sites_func_lawoffice_partymanagement_dev_name_resource
  name: '${sites_func_lawoffice_partymanagement_dev_name}.azurewebsites.net'
  location: 'West Europe'
  properties: {
    siteName: 'func-lawoffice-partymanagement-dev'
    hostNameType: 'Verified'
  }
}

resource staticSites_swaf_lawoffice_dev_name_default 'Microsoft.Web/staticSites/basicAuth@2024-11-01' = {
  parent: staticSites_swaf_lawoffice_dev_name_resource
  name: 'default'
  location: 'West Europe'
  properties: {
    applicableEnvironmentsMode: 'SpecifiedEnvironments'
  }
}

resource service_apim_lawoffice_dev_name_CaseManagement_delete_deletecase 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'delete-deletecase'
  properties: {
    displayName: 'DeleteCase'
    method: 'DELETE'
    urlTemplate: '/case/{officeId}/{caseId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_delete_deletecase_type
      }
      {
        name: 'caseId'
        required: true
        values: []
        type: operations_delete_deletecase_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_delete_deletedocumentfile 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'delete-deletedocumentfile'
  properties: {
    displayName: 'DeleteDocumentFile'
    method: 'DELETE'
    urlTemplate: '/documentFile/{officeId}/{documentFileId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_delete_deletedocumentfile_type
      }
      {
        name: 'documentFileId'
        required: true
        values: []
        type: operations_delete_deletedocumentfile_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_delete_deletehearing 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'delete-deletehearing'
  properties: {
    displayName: 'DeleteHearing'
    method: 'DELETE'
    urlTemplate: '/hearing/{officeId}/{hearingId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_delete_deletehearing_type
      }
      {
        name: 'hearingId'
        required: true
        values: []
        type: operations_delete_deletehearing_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getallcases 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'get-getallcases'
  properties: {
    displayName: 'GetAllCases'
    method: 'GET'
    urlTemplate: '/case/{officeId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getallcases_type
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getallclients 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'get-getallclients'
  properties: {
    displayName: 'GetAllClients'
    method: 'GET'
    urlTemplate: '/client/{officeId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getallclients_type
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getalldocumentfiles 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'get-getalldocumentfiles'
  properties: {
    displayName: 'GetAllDocumentFiles'
    method: 'GET'
    urlTemplate: '/documentFile/{officeId}/case/{caseId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getalldocumentfiles_type
      }
      {
        name: 'caseId'
        required: true
        values: []
        type: operations_get_getalldocumentfiles_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getallhearings 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'get-getallhearings'
  properties: {
    displayName: 'GetAllHearings'
    method: 'GET'
    urlTemplate: '/hearing/{officeId}/case/{caseId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getallhearings_type
      }
      {
        name: 'caseId'
        required: true
        values: []
        type: operations_get_getallhearings_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_get_getalllawyers 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'get-getalllawyers'
  properties: {
    displayName: 'GetAllLawyers'
    method: 'GET'
    urlTemplate: '/lawyer/{officeId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getalllawyers_type
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getallopposingparties 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'get-getallopposingparties'
  properties: {
    displayName: 'GetAllOpposingParties'
    method: 'GET'
    urlTemplate: '/opposingParty/{officeId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getallopposingparties_type
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getcase 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'get-getcase'
  properties: {
    displayName: 'GetCase'
    method: 'GET'
    urlTemplate: '/case/{officeId}/{caseId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getcase_type
      }
      {
        name: 'caseId'
        required: true
        values: []
        type: operations_get_getcase_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getclient 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'get-getclient'
  properties: {
    displayName: 'GetClient'
    method: 'GET'
    urlTemplate: '/client/{officeId}/{clientId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getclient_type
      }
      {
        name: 'clientId'
        required: true
        values: []
        type: operations_get_getclient_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getdocumentfile 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'get-getdocumentfile'
  properties: {
    displayName: 'GetDocumentFile'
    method: 'GET'
    urlTemplate: '/documentFile/{officeId}/{documentFileId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getdocumentfile_type
      }
      {
        name: 'documentFileId'
        required: true
        values: []
        type: operations_get_getdocumentfile_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_gethearing 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'get-gethearing'
  properties: {
    displayName: 'GetHearing'
    method: 'GET'
    urlTemplate: '/hearing/{officeId}/{hearingId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_gethearing_type
      }
      {
        name: 'hearingId'
        required: true
        values: []
        type: operations_get_gethearing_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_get_getlawyer 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'get-getlawyer'
  properties: {
    displayName: 'GetLawyer'
    method: 'GET'
    urlTemplate: '/lawyer/{officeId}/{lawyerId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getlawyer_type
      }
      {
        name: 'lawyerId'
        required: true
        values: []
        type: operations_get_getlawyer_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_get_getoffice 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'get-getoffice'
  properties: {
    displayName: 'GetOffice'
    method: 'GET'
    urlTemplate: '/office/{officeId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getoffice_type
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getopposingparty 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'get-getopposingparty'
  properties: {
    displayName: 'GetOpposingParty'
    method: 'GET'
    urlTemplate: '/opposingParty/{officeId}/{opposingPartyId}'
    templateParameters: [
      {
        name: 'officeId'
        required: true
        values: []
        type: operations_get_getopposingparty_type
      }
      {
        name: 'opposingPartyId'
        required: true
        values: []
        type: operations_get_getopposingparty_type_1
      }
    ]
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_post_postcase 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'post-postcase'
  properties: {
    displayName: 'PostCase'
    method: 'POST'
    urlTemplate: '/case'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_post_postclient 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'post-postclient'
  properties: {
    displayName: 'PostClient'
    method: 'POST'
    urlTemplate: '/client'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_post_postdocumentfile 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'post-postdocumentfile'
  properties: {
    displayName: 'PostDocumentFile'
    method: 'POST'
    urlTemplate: '/documentFile'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_post_posthearing 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'post-posthearing'
  properties: {
    displayName: 'PostHearing'
    method: 'POST'
    urlTemplate: '/hearing'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_post_postlawyer 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'post-postlawyer'
  properties: {
    displayName: 'PostLawyer'
    method: 'POST'
    urlTemplate: '/lawyer'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_post_postoffice 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'post-postoffice'
  properties: {
    displayName: 'PostOffice'
    method: 'POST'
    urlTemplate: '/office'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_post_postopposingparty 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'post-postopposingparty'
  properties: {
    displayName: 'PostOpposingParty'
    method: 'POST'
    urlTemplate: '/opposingParty'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_put_putcase 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'put-putcase'
  properties: {
    displayName: 'PutCase'
    method: 'PUT'
    urlTemplate: '/case'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_put_putclient 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'put-putclient'
  properties: {
    displayName: 'PutClient'
    method: 'PUT'
    urlTemplate: '/client'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_put_putdocumentfile 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'put-putdocumentfile'
  properties: {
    displayName: 'PutDocumentFile'
    method: 'PUT'
    urlTemplate: '/documentFile'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_put_puthearing 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'put-puthearing'
  properties: {
    displayName: 'PutHearing'
    method: 'PUT'
    urlTemplate: '/hearing'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_put_putlawyer 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'put-putlawyer'
  properties: {
    displayName: 'PutLawyer'
    method: 'PUT'
    urlTemplate: '/lawyer'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_put_putoffice 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'put-putoffice'
  properties: {
    displayName: 'PutOffice'
    method: 'PUT'
    urlTemplate: '/office'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_put_putopposingparty 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'put-putopposingparty'
  properties: {
    displayName: 'PutOpposingParty'
    method: 'PUT'
    urlTemplate: '/opposingParty'
    templateParameters: []
    responses: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_policy 'Microsoft.ApiManagement/service/apis/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'policy'
  properties: {
    value: '<!--\r\n    - Policies are applied in the order they appear.\r\n    - Position <base/> inside a section to inherit policies from the outer scope.\r\n    - Comments within policies are not preserved.\r\n-->\r\n<!-- Add policies as children to the <inbound>, <outbound>, <backend>, and <on-error> elements -->\r\n<policies>\r\n  <!-- Throttle, authorize, validate, cache, or transform the requests -->\r\n  <inbound>\r\n    <base />\r\n    <set-header name="x-functions-key" exists-action="override">\r\n      <value>{{func-lawoffice-casemanagement-dev-key}}</value>\r\n    </set-header>\r\n  </inbound>\r\n  <!-- Control if and how the requests are forwarded to services  -->\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <!-- Customize the responses -->\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <!-- Handle exceptions and customize error responses  -->\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_policy 'Microsoft.ApiManagement/service/apis/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'policy'
  properties: {
    value: '<!--\r\n    - Policies are applied in the order they appear.\r\n    - Position <base/> inside a section to inherit policies from the outer scope.\r\n    - Comments within policies are not preserved.\r\n-->\r\n<!-- Add policies as children to the <inbound>, <outbound>, <backend>, and <on-error> elements -->\r\n<policies>\r\n  <!-- Throttle, authorize, validate, cache, or transform the requests -->\r\n  <inbound>\r\n    <base />\r\n    <set-header name="x-functions-key" exists-action="override">\r\n      <value>{{func-lawoffice-officemanagement-dev-key}}</value>\r\n    </set-header>\r\n  </inbound>\r\n  <!-- Control if and how the requests are forwarded to services  -->\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <!-- Customize the responses -->\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <!-- Handle exceptions and customize error responses  -->\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_policy 'Microsoft.ApiManagement/service/apis/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'policy'
  properties: {
    value: '<!--\r\n    - Policies are applied in the order they appear.\r\n    - Position <base/> inside a section to inherit policies from the outer scope.\r\n    - Comments within policies are not preserved.\r\n-->\r\n<!-- Add policies as children to the <inbound>, <outbound>, <backend>, and <on-error> elements -->\r\n<policies>\r\n  <!-- Throttle, authorize, validate, cache, or transform the requests -->\r\n  <inbound>\r\n    <base />\r\n    <set-header name="x-functions-key" exists-action="override">\r\n      <value>{{func-lawoffice-partymanagement-dev-key}}</value>\r\n    </set-header>\r\n  </inbound>\r\n  <!-- Control if and how the requests are forwarded to services  -->\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <!-- Customize the responses -->\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <!-- Handle exceptions and customize error responses  -->\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_default 'Microsoft.ApiManagement/service/apis/wikis@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement
  name: 'default'
  properties: {
    documents: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_default 'Microsoft.ApiManagement/service/apis/wikis@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement
  name: 'default'
  properties: {
    documents: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_default 'Microsoft.ApiManagement/service/apis/wikis@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement
  name: 'default'
  properties: {
    documents: []
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement_cases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement
  name: 'cases'
  properties: {
    resource: {
      id: 'cases'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/officeId'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_partymanagement_clients 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_partymanagement
  name: 'clients'
  properties: {
    resource: {
      id: 'clients'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/officeId'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement_documentfiles 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement
  name: 'documentfiles'
  properties: {
    resource: {
      id: 'documentfiles'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/officeId'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement_hearings 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_casemanagement
  name: 'hearings'
  properties: {
    resource: {
      id: 'hearings'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/officeId'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_officemanagement_lawyers 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_officemanagement
  name: 'lawyers'
  properties: {
    resource: {
      id: 'lawyers'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/officeId'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_officemanagement_offices 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_officemanagement
  name: 'offices'
  properties: {
    resource: {
      id: 'offices'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/id'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource databaseAccounts_cos_lawoffice_officemanagement_dev_name_partymanagement_opposingparties 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-05-01-preview' = {
  parent: databaseAccounts_cos_lawoffice_officemanagement_dev_name_partymanagement
  name: 'opposingparties'
  properties: {
    resource: {
      id: 'opposingparties'
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/officeId'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      computedProperties: []
    }
  }
  dependsOn: [
    databaseAccounts_cos_lawoffice_officemanagement_dev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_59769a1c_3e26_4523_a6b5_6040e5b49edb 'Microsoft.Storage/storageAccounts/blobServices/containers@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_default
  name: '59769a1c-3e26-4523-a6b5-6040e5b49edb'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_azure_webjobs_hosts 'Microsoft.Storage/storageAccounts/blobServices/containers@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_default
  name: 'azure-webjobs-hosts'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_azure_webjobs_secrets 'Microsoft.Storage/storageAccounts/blobServices/containers@2025-01-01' = {
  parent: storageAccounts_rglawofficedev_name_default
  name: 'azure-webjobs-secrets'
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_func_lawoffice_casemanagement_dev826a 'Microsoft.Storage/storageAccounts/fileServices/shares@2025-01-01' = {
  parent: Microsoft_Storage_storageAccounts_fileServices_storageAccounts_rglawofficedev_name_default
  name: 'func-lawoffice-casemanagement-dev826a'
  properties: {
    accessTier: 'TransactionOptimized'
    shareQuota: 102400
    enabledProtocols: 'SMB'
  }
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_func_lawoffice_officemanagement_dev83eb 'Microsoft.Storage/storageAccounts/fileServices/shares@2025-01-01' = {
  parent: Microsoft_Storage_storageAccounts_fileServices_storageAccounts_rglawofficedev_name_default
  name: 'func-lawoffice-officemanagement-dev83eb'
  properties: {
    accessTier: 'TransactionOptimized'
    shareQuota: 102400
    enabledProtocols: 'SMB'
  }
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_func_lawoffice_partymanagement_dev8792 'Microsoft.Storage/storageAccounts/fileServices/shares@2025-01-01' = {
  parent: Microsoft_Storage_storageAccounts_fileServices_storageAccounts_rglawofficedev_name_default
  name: 'func-lawoffice-partymanagement-dev8792'
  properties: {
    accessTier: 'TransactionOptimized'
    shareQuota: 102400
    enabledProtocols: 'SMB'
  }
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource storageAccounts_rglawofficedev_name_default_AzureFunctionsDiagnosticEvents202602 'Microsoft.Storage/storageAccounts/tableServices/tables@2025-01-01' = {
  parent: Microsoft_Storage_storageAccounts_tableServices_storageAccounts_rglawofficedev_name_default
  name: 'AzureFunctionsDiagnosticEvents202602'
  properties: {}
  dependsOn: [
    storageAccounts_rglawofficedev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_delete_deletecase_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_delete_deletecase
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_delete_deletedocumentfile_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_delete_deletedocumentfile
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_delete_deletehearing_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_delete_deletehearing
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getallcases_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_get_getallcases
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getalldocumentfiles_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_get_getalldocumentfiles
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getallhearings_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_get_getallhearings
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getcase_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_get_getcase
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_getdocumentfile_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_get_getdocumentfile
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_get_gethearing_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_get_gethearing
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_post_postcase_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_post_postcase
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_post_postdocumentfile_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_post_postdocumentfile
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_post_posthearing_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_post_posthearing
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_put_putcase_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_put_putcase
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_put_putdocumentfile_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_put_putdocumentfile
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_CaseManagement_put_puthearing_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_CaseManagement_put_puthearing
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-casemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_CaseManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_get_getalllawyers_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_get_getalllawyers
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_get_getlawyer_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_get_getlawyer
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_get_getoffice_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_get_getoffice
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_post_postlawyer_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_post_postlawyer
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_post_postoffice_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_post_postoffice
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_put_putlawyer_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_put_putlawyer
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_OfficeManagement_put_putoffice_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_OfficeManagement_put_putoffice
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-officemanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_OfficeManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getallclients_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_get_getallclients
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getallopposingparties_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_get_getallopposingparties
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getclient_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_get_getclient
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_get_getopposingparty_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_get_getopposingparty
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_post_postclient_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_post_postclient
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_post_postopposingparty_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_post_postopposingparty
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_put_putclient_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_put_putclient
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}

resource service_apim_lawoffice_dev_name_PartyManagement_put_putopposingparty_policy 'Microsoft.ApiManagement/service/apis/operations/policies@2024-06-01-preview' = {
  parent: service_apim_lawoffice_dev_name_PartyManagement_put_putopposingparty
  name: 'policy'
  properties: {
    value: '<policies>\r\n  <inbound>\r\n    <base />\r\n    <set-backend-service id="apim-generated-policy" backend-id="func-lawoffice-partymanagement-dev" />\r\n  </inbound>\r\n  <backend>\r\n    <base />\r\n  </backend>\r\n  <outbound>\r\n    <base />\r\n  </outbound>\r\n  <on-error>\r\n    <base />\r\n  </on-error>\r\n</policies>'
    format: 'xml'
  }
  dependsOn: [
    service_apim_lawoffice_dev_name_PartyManagement
    service_apim_lawoffice_dev_name_resource
  ]
}
