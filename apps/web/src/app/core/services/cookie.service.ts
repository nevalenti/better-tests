import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CookieService {
  setCookie(name: string, value: string, days = 365): void {
    try {
      const date = new Date();
      date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
      const expires = 'expires=' + date.toUTCString();
      const secure = window.location.protocol === 'https:' ? 'Secure;' : '';
      document.cookie = `${name}=${encodeURIComponent(value)};${expires};path=/;${secure}SameSite=Strict`;
    } catch (error) {
      console.error('Error setting cookie:', error);
    }
  }

  getCookie(name: string): string | null {
    try {
      const nameEQ = name + '=';
      const cookies = document.cookie.split(';');
      for (let cookie of cookies) {
        cookie = cookie.trim();
        if (cookie.startsWith(nameEQ)) {
          return decodeURIComponent(cookie.substring(nameEQ.length));
        }
      }
      return null;
    } catch (error) {
      console.error('Error reading cookie:', error);
      return null;
    }
  }

  removeCookie(name: string): void {
    try {
      document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;`;
    } catch (error) {
      console.error('Error removing cookie:', error);
    }
  }
}
