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
  REDIRECT_URL?: string;
};

const DEFAULT_API_BASE_URL: ApiBaseUrlConfig = {
  OFFICE_MANAGEMENT: 'http://localhost:7206/api',
  PARTY_MANAGEMENT: 'http://localhost:7207/api',
  CASE_MANAGEMENT: 'http://localhost:7208/api'
};

const DEFAULT_REDIRECT_URL = 'http://localhost:4200';

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

const getRedirectUrl = (): string => {
  return getRuntimeEnv().REDIRECT_URL ?? DEFAULT_REDIRECT_URL;
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

export const REDIRECT_URL = getRedirectUrl();

export const API_ENDPOINTS = {
  GET_OFFICE: `/office`,
  UPDATE_OFFICE: '/office',

  GET_CLIENTS: `/client`,
  GET_CLIENT: (clientId: string) => `/client/${clientId}`,
  CREATE_CLIENT: '/client',
  UPDATE_CLIENT: '/client',

  GET_OPPOSING_PARTIES: `/opposingParty`,
  GET_OPPOSING_PARTY: (opposingPartyId: string) => `/opposingParty/${opposingPartyId}`,
  CREATE_OPPOSING_PARTY: '/opposingParty',
  UPDATE_OPPOSING_PARTY: '/opposingParty',

  GET_CASES: `/case`,
  GET_CASE: (caseId: string) => `/case/${caseId}`,
  CREATE_CASE: '/case',
  UPDATE_CASE: '/case'
};
