using './main.bicep'

param environmentName = 'test'
param apimPublisherEmail = 'dejan.efremov@outlook.com'

param tags = {
  project: 'LawOffice'
  env: 'test'
  managedBy: 'bicep'
}

// All resource names use deterministic defaults derived from environmentName = 'test':
//   Storage:   stlawofficetest
//   Cosmos DB: cos-lawoffice-test
//   APIM:      apim-lawoffice-test
//   SWA:       swa-lawoffice-portal-test
//   Functions: func-lawoffice-{service}-test

// Override names to match existing TEST resources
param storageAccountName = 'stlawofficetestshared'
param cosmosAccountName = 'cos-lawoffice-officemanagement-test'
param staticWebAppName = 'purple-desert-0ca068003'

// SWA GitHub integration
param staticWebAppRepositoryUrl = 'https://github.com/dejanefremovout/LawOffice'
param staticWebAppBranch = 'test'

// Same Entra ID CIAM tenant as DEV
param jwtOpenIdConfigUrl = 'https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0/.well-known/openid-configuration'
param jwtAudience = 'a9a5990c-f11e-49df-a582-a2c1416456cf'
param jwtIssuer = 'https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0'
