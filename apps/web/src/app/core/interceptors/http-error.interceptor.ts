import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';

import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse | unknown) => {
      if (error instanceof HttpErrorResponse) {
        const message =
          error.error?.detail ||
          error.error?.message ||
          error.message ||
          'Error';

        return throwError(() => ({
          status: error.status,
          message,
          error: error.error,
        }));
      }

      return throwError(() => ({
        status: 0,
        statusText: 'Network Error',
        message: 'Failed to reach the server',
        error,
      }));
    }),
  );
};
