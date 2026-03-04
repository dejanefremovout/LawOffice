// import { HttpInterceptorFn } from '@angular/common/http';

// export const apimSimulatorInterceptor: HttpInterceptorFn = (req, next) => {
//   return next(req);
// };


import { Injectable, inject } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { MsalService } from '@azure/msal-angular';
import { jwtDecode } from 'jwt-decode';

@Injectable()
export class ApimSimulatorInterceptor implements HttpInterceptor {
  private readonly authService = inject(MsalService);
  private readonly apiScopes = ['api://a9a5990c-f11e-49df-a582-a2c1416456cf/access_as_user'];

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    // 1. TRIGGER: Only run this if we are targeting Localhost
    const isLocalApi = request.url.startsWith('http://localhost') || request.url.startsWith('https://localhost'); 
    if (!isLocalApi) {
      return next.handle(request);
    }

    // 2. GET TOKEN: Retrieve the active access token
    const account = this.authService.instance.getActiveAccount() ?? this.authService.instance.getAllAccounts()[0];
    if (!account) {
      return next.handle(request);
    }

    return from(
      this.authService.instance.acquireTokenSilent({
        account,
        scopes: this.apiScopes
      })
    ).pipe(
      switchMap((result) => {
        const accessToken = result.accessToken;
        if (!accessToken) {
          return next.handle(request);
        }

        try {
          const decoded = jwtDecode<Record<string, unknown>>(accessToken);
          const officeId = decoded['extension_OfficeId'];

          if (typeof officeId === 'string' && officeId.length > 0) {
            const cloned = request.clone({
              setHeaders: {
                'X-Office-Id': officeId
              }
            });
            return next.handle(cloned);
          }
        } catch {
          console.warn('Local APIM simulation failed to decode access token.');
        }

        return next.handle(request);
      }),
      catchError(() => next.handle(request))
    );
  }
}
