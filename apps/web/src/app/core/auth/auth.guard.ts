import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import Keycloak from 'keycloak-js';

export const authGuard: CanActivateFn = async (_route, state) => {
  const keycloak = inject(Keycloak);
  const router = inject(Router);

  try {
    if (!keycloak.authenticated) {
      keycloak.login({
        redirectUri: window.location.origin + state.url,
      });
      return false;
    }
    return true;
  } catch (error) {
    console.error('Auth guard error:', error);
    router.navigate(['/login']);
    return false;
  }
};
