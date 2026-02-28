import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import Keycloak from 'keycloak-js';

import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle.component';

@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  imports: [RouterLink, ThemeToggleComponent],
})
export class HeaderComponent {
  protected keycloak = inject(Keycloak);

  closeMobileMenu(): void {
    const drawerToggle = document.getElementById(
      'mobile-nav-drawer',
    ) as HTMLInputElement;
    if (drawerToggle) {
      drawerToggle.checked = false;
    }
  }

  login(): void {
    this.keycloak.login();
  }

  register(): void {
    this.keycloak.register();
  }

  logout(): void {
    this.keycloak.logout({ redirectUri: window.location.origin });
  }
}
