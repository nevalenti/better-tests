import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

import Keycloak from 'keycloak-js';

import { FooterComponent } from './layout/footer/footer.component';
import { HeaderComponent } from './layout/header/header.component';
import { CookiesComponent } from './shared/components/cookies/cookies.component';
import { NotificationsComponent } from './shared/components/notifications/notifications.component';

@Component({
  imports: [
    RouterModule,
    HeaderComponent,
    FooterComponent,
    CookiesComponent,
    NotificationsComponent,
  ],
  selector: 'app-root',
  templateUrl: 'app.html',
})
export class App implements OnInit {
  protected title = 'web';
  protected keycloak = inject(Keycloak);

  ngOnInit(): void {
    if (this.keycloak.authenticated) {
      console.log('User Info:', this.keycloak.tokenParsed);
      console.log(
        'Username:',
        this.keycloak.tokenParsed?.['preferred_username'],
      );
      console.log('Email:', this.keycloak.tokenParsed?.['email']);
      console.log('Realm Roles:', this.keycloak.realmAccess?.roles);
    } else {
      console.log('User is not authenticated');
    }
  }
}
