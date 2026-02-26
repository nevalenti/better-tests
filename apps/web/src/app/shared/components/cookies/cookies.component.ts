import { Component, computed, inject } from '@angular/core';

import { CookieConsentService } from '../../../core/services/cookie-consent.service';

@Component({
  selector: 'app-cookies',
  templateUrl: 'cookies.component.html',
})
export class CookiesComponent {
  private cookieConsentService = inject(CookieConsentService);

  showBanner = computed(
    () => this.cookieConsentService.consentStatus() === null,
  );

  acceptCookies = () => this.cookieConsentService.acceptCookies();
  declineCookies = () => this.cookieConsentService.declineCookies();
}
