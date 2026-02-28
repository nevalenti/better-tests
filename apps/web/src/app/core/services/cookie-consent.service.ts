import {
  afterNextRender,
  computed,
  Injectable,
  inject,
  signal,
} from '@angular/core';

import { CookieService } from './cookie.service';

@Injectable({
  providedIn: 'root',
})
export class CookieConsentService {
  private readonly CONSENT_COOKIE_KEY = 'cookies-consent';

  private cookieService = inject(CookieService);
  private consentStatusSignal = signal<boolean | null>(null);

  consentStatus = this.consentStatusSignal.asReadonly();
  hasAccepted = computed(() => this.consentStatus() === true);
  hasDeclined = computed(() => this.consentStatus() === false);
  hasResponded = computed(() => this.consentStatus() !== null);

  constructor() {
    afterNextRender(() => {
      this.consentStatusSignal.set(this.getStoredConsent());
    });
  }

  private getStoredConsent(): boolean | null {
    try {
      const stored = this.cookieService.getCookie(this.CONSENT_COOKIE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch (error) {
      console.error('Error reading cookie consent:', error);
      return null;
    }
  }

  acceptCookies(): void {
    try {
      this.cookieService.setCookie(
        this.CONSENT_COOKIE_KEY,
        JSON.stringify(true),
      );
      this.consentStatusSignal.set(true);
    } catch (error) {
      console.error('Error saving cookie consent:', error);
    }
  }

  declineCookies(): void {
    try {
      this.cookieService.setCookie(
        this.CONSENT_COOKIE_KEY,
        JSON.stringify(false),
      );
      this.consentStatusSignal.set(false);
    } catch (error) {
      console.error('Error saving cookie consent:', error);
    }
  }

  resetConsent(): void {
    try {
      this.cookieService.removeCookie(this.CONSENT_COOKIE_KEY);
      this.consentStatusSignal.set(null);
    } catch (error) {
      console.error('Error removing cookie consent:', error);
    }
  }
}
