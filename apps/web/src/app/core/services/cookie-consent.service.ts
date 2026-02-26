import { afterNextRender, computed, Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CookieConsentService {
  private readonly CONSENT_STORAGE_KEY = 'cookies-consent';

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
      const stored = localStorage.getItem(this.CONSENT_STORAGE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch (error) {
      console.error('Error reading cookie consent:', error);
      return null;
    }
  }

  acceptCookies(): void {
    try {
      localStorage.setItem(this.CONSENT_STORAGE_KEY, JSON.stringify(true));
      this.consentStatusSignal.set(true);
    } catch (error) {
      console.error('Error saving cookie consent:', error);
    }
  }

  declineCookies(): void {
    try {
      localStorage.setItem(this.CONSENT_STORAGE_KEY, JSON.stringify(false));
      this.consentStatusSignal.set(false);
    } catch (error) {
      console.error('Error saving cookie consent:', error);
    }
  }

  resetConsent(): void {
    try {
      localStorage.removeItem(this.CONSENT_STORAGE_KEY);
      this.consentStatusSignal.set(null);
    } catch (error) {
      console.error('Error removing cookie consent:', error);
    }
  }
}
