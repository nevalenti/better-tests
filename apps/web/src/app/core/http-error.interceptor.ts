import { HttpInterceptorFn } from '@angular/common/http';

import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

const STATUS_MAP: { [key: number]: string } = {
  400: 'Bad Request',
  401: 'Unauthorized',
  403: 'Forbidden',
  404: 'Not Found',
  409: 'Conflict',
  500: 'Internal Server Error',
  502: 'Bad Gateway',
  503: 'Service Unavailable',
};

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error) => {
      const statusText =
        STATUS_MAP[error.status] || error.statusText || 'Unknown Error';
      const enrichedError = {
        ...error,
        status: error.status,
        statusText: statusText,
        message: error.error?.detail || error.message || statusText,
      };

      return throwError(() => enrichedError);
    }),
  );
};
