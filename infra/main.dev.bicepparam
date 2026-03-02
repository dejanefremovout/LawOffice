using './main.bicep'

param environmentName = 'dev'
param apimPublisherEmail = 'dejan.efremov@outlook.com'

param tags = {
  project: 'LawOffice'
  env: 'dev'
  managedBy: 'bicep'
}

// Override names to match existing DEV resources
param storageAccountName = 'stlawofficedevshared'
param cosmosAccountName = 'cos-lawoffice-officemanagement-dev'
param staticWebAppName = 'green-sea-058b76203'

// SWA GitHub integration
param staticWebAppRepositoryUrl = 'https://github.com/dejanefremovout/LawOffice'
param staticWebAppBranch = 'master'

// Entra ID CIAM for JWT validation
param jwtOpenIdConfigUrl = 'https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0/.well-known/openid-configuration'
param jwtAudience = 'a9a5990c-f11e-49df-a582-a2c1416456cf'
param jwtIssuer = 'https://lawofficecustomers.ciamlogin.com/f3863a43-68a6-4422-9e12-a14fd7e45a7f/v2.0'
