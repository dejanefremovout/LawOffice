import { ApplicationConfig, provideBrowserGlobalErrorListeners, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { BrowserCacheLocation, IPublicClientApplication, PublicClientApplication, InteractionType } from '@azure/msal-browser';
import { MsalModule, MSAL_INSTANCE, MSAL_GUARD_CONFIG, MsalGuardConfiguration, MsalService, MsalGuard, MsalBroadcastService, MsalInterceptor, MSAL_INTERCEPTOR_CONFIG, MsalInterceptorConfiguration } from '@azure/msal-angular';


import { routes } from './app.routes';
import { API_BASE_URL, REDIRECT_URL } from './constants/api.constants';
import { ApimSimulatorInterceptor } from './apim-simulator-interceptor';

export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: 'a9a5990c-f11e-49df-a582-a2c1416456cf',
      authority: 'https://lawofficecustomers.ciamlogin.com',
      redirectUri: REDIRECT_URL,
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
    },
  });
}

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: ['openid', 'profile'],
    },
  };
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();

  const scopes = ['api://a9a5990c-f11e-49df-a582-a2c1416456cf/access_as_user'];
  const addProtectedResource = (baseUrl: string): void => {
    const normalizedBaseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    protectedResourceMap.set(normalizedBaseUrl, scopes);
    protectedResourceMap.set(`${normalizedBaseUrl}/*`, scopes);
  };

  addProtectedResource(API_BASE_URL.OFFICE_MANAGEMENT);
  addProtectedResource(API_BASE_URL.PARTY_MANAGEMENT);
  addProtectedResource(API_BASE_URL.CASE_MANAGEMENT);

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap,
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    importProvidersFrom(MsalModule),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: MSALGuardConfigFactory,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ApimSimulatorInterceptor,
      multi: true
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService,
  ]
};
