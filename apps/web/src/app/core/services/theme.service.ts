import { afterNextRender, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly THEME_KEY = 'app-theme';
  private readonly LIGHT_THEME = 'emerald';
  private readonly DARK_THEME = 'dim';
  private readonly DEFAULT_THEME = this.DARK_THEME;

  constructor() {
    afterNextRender(() => {
      this.loadTheme();
    });
  }

  loadTheme(): void {
    const savedTheme =
      localStorage.getItem(this.THEME_KEY) || this.DEFAULT_THEME;
    this.setTheme(savedTheme);
  }

  setTheme(theme: string): void {
    localStorage.setItem(this.THEME_KEY, theme);
    document.documentElement.setAttribute('data-theme', theme);
  }

  toggleTheme(): void {
    const currentTheme =
      localStorage.getItem(this.THEME_KEY) || this.DEFAULT_THEME;
    const newTheme =
      currentTheme === this.LIGHT_THEME ? this.DARK_THEME : this.LIGHT_THEME;
    this.setTheme(newTheme);
  }

  getCurrentTheme(): string {
    if (typeof localStorage === 'undefined') return this.DEFAULT_THEME;
    return localStorage.getItem(this.THEME_KEY) || this.DEFAULT_THEME;
  }
}
