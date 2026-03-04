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
import { UserService } from './services/user.service';

@Injectable()
export class ApimSimulatorInterceptor implements HttpInterceptor {
  private readonly userService = inject(UserService);

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    // 1. TRIGGER: Only run this if we are targeting Localhost
    const isLocalApi = request.url.startsWith('http://localhost') || request.url.startsWith('https://localhost'); 
    if (!isLocalApi) {
      return next.handle(request);
    }

    // 2. GET OFFICE ID: Resolve from access token via UserService
    return from(this.userService.getOfficeId()).pipe(
      switchMap((officeId) => {
        if (officeId) {
          const cloned = request.clone({
            setHeaders: {
              'X-Office-Id': officeId
            }
          });
          return next.handle(cloned);
        }

        return next.handle(request);
      }),
      catchError(() => next.handle(request))
    );
  }
}
