/**
 * API Configuration Constants
 */

type ApiBaseUrlConfig = {
  OFFICE_MANAGEMENT: string;
  PARTY_MANAGEMENT: string;
  CASE_MANAGEMENT: string;
};

type RuntimeEnvConfig = {
  API_BASE_URL?: Partial<ApiBaseUrlConfig>;
};

const DEFAULT_API_BASE_URL: ApiBaseUrlConfig = {
  OFFICE_MANAGEMENT: 'http://localhost:7206',
  PARTY_MANAGEMENT: 'http://localhost:7207',
  CASE_MANAGEMENT: 'http://localhost:7208'
};

const getRuntimeEnv = (): RuntimeEnvConfig => {
  const globalConfig = (globalThis as { __env?: RuntimeEnvConfig }).__env;

  return globalConfig ?? {};
};

const getApiBaseUrl = (): ApiBaseUrlConfig => {
  const runtimeApiBaseUrl = getRuntimeEnv().API_BASE_URL;

  return {
    OFFICE_MANAGEMENT: runtimeApiBaseUrl?.OFFICE_MANAGEMENT ?? DEFAULT_API_BASE_URL.OFFICE_MANAGEMENT,
    PARTY_MANAGEMENT: runtimeApiBaseUrl?.PARTY_MANAGEMENT ?? DEFAULT_API_BASE_URL.PARTY_MANAGEMENT,
    CASE_MANAGEMENT: runtimeApiBaseUrl?.CASE_MANAGEMENT ?? DEFAULT_API_BASE_URL.CASE_MANAGEMENT
  };
};

export const API_BASE_URL = {
  get OFFICE_MANAGEMENT(): string {
    return getApiBaseUrl().OFFICE_MANAGEMENT;
  },
  get PARTY_MANAGEMENT(): string {
    return getApiBaseUrl().PARTY_MANAGEMENT;
  },
  get CASE_MANAGEMENT(): string {
    return getApiBaseUrl().CASE_MANAGEMENT;
  }
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
