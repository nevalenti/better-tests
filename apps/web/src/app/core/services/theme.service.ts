import { afterNextRender, Injectable, inject } from '@angular/core';

import { CookieService } from './cookie.service';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly THEME_KEY = 'app-theme';
  private readonly LIGHT_THEME = 'emerald';
  private readonly DARK_THEME = 'dim';
  private readonly DEFAULT_THEME = this.DARK_THEME;

  private cookieService = inject(CookieService);

  constructor() {
    afterNextRender(() => {
      this.loadTheme();
    });
  }

  loadTheme(): void {
    const savedTheme =
      this.cookieService.getCookie(this.THEME_KEY) || this.DEFAULT_THEME;
    this.setTheme(savedTheme);
  }

  setTheme(theme: string): void {
    this.cookieService.setCookie(this.THEME_KEY, theme);
    document.documentElement.setAttribute('data-theme', theme);
  }

  toggleTheme(): void {
    const currentTheme =
      this.cookieService.getCookie(this.THEME_KEY) || this.DEFAULT_THEME;
    const newTheme =
      currentTheme === this.LIGHT_THEME ? this.DARK_THEME : this.LIGHT_THEME;
    this.setTheme(newTheme);
  }

  getCurrentTheme(): string {
    return this.cookieService.getCookie(this.THEME_KEY) || this.DEFAULT_THEME;
  }
}
