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
    // Only run this behavior for local API endpoints.
    const isLocalApi =
      request.url.startsWith('http://localhost') ||
      request.url.startsWith('https://localhost') ||
      request.url.startsWith('http://127.0.0.1') ||
      request.url.startsWith('https://127.0.0.1');

    if (!isLocalApi) {
      return next.handle(request);
    }

    const withOfficeHeader = (officeId: string) =>
      request.clone({
        setHeaders: {
          'X-Office-Id': officeId
        }
      });

    // Resolve office id from token; if absent, forward request unchanged so backend fails as designed.
    return from(this.userService.getOfficeId()).pipe(
      switchMap((officeId) => {
        const resolvedOfficeId = officeId?.trim();

        if (!resolvedOfficeId) {
          return next.handle(request);
        }

        return next.handle(withOfficeHeader(resolvedOfficeId));
      }),
      catchError(() => next.handle(request))
    );
  }
}
