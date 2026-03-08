using './main.bicep'

param environmentName = 'master'
param apimPublisherEmail = 'dejan.efremov@outlook.com'

param tags = {
  project: 'LawOffice'
  env: 'master'
  managedBy: 'bicep'
}

// Resource names for MASTER environment
param storageAccountName = 'stlawofficemastershared'
param cosmosAccountName = 'cos-lawoffice-officemanagement-master'
param staticWebAppName = 'swa-lawoffice-portal-master'

// SWA GitHub integration
param staticWebAppRepositoryUrl = 'https://github.com/dejanefremovout/LawOffice'
param staticWebAppBranch = 'master'

// Same Entra ID CIAM tenant as DEV/TEST
param jwtOpenIdConfigUrl = 'https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0/.well-known/openid-configuration'
param jwtAudience = 'a9a5990c-f11e-49df-a582-a2c1416456cf'
param jwtIssuer = 'https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0'
