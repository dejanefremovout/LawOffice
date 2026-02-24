/**
 * API Configuration Constants
 */

export const API_BASE_URL = {
  OFFICE_MANAGEMENT: 'https://func-lawoffice-officemanagement-dev.azurewebsites.net',
  PARTY_MANAGEMENT: 'https://func-lawoffice-partymanagement-dev.azurewebsites.net',
  CASE_MANAGEMENT: 'https://func-lawoffice-casemanagement-dev.azurewebsites.net'
};

export const API_ENDPOINTS = {
  GET_OFFICE: (officeId: string) => `/api/office/${officeId}`,
  CREATE_OFFICE: '/api/office',
  UPDATE_OFFICE: '/api/office',

  GET_CLIENTS: (officeId: string) => `/api/client/${officeId}`,
  GET_CLIENT: (officeId: string, clientId: string) => `/api/client/${officeId}/${clientId}`,
  CREATE_CLIENT: '/api/client',
  UPDATE_CLIENT: '/api/client',

  GET_OPPOSING_PARTIES: (officeId: string) => `/api/opposingParty/${officeId}`,
  GET_OPPOSING_PARTY: (officeId: string, opposingPartyId: string) => `/api/opposingParty/${officeId}/${opposingPartyId}`,
  CREATE_OPPOSING_PARTY: '/api/opposingParty',
  UPDATE_OPPOSING_PARTY: '/api/opposingParty',

  GET_CASES: (officeId: string) => `/api/case/${officeId}`,
  GET_CASE: (officeId: string, caseId: string) => `/api/case/${officeId}/${caseId}`,
  CREATE_CASE: '/api/case',
  UPDATE_CASE: '/api/case'
};
