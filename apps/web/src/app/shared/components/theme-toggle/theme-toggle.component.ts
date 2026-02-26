import { Component, computed, inject } from '@angular/core';

import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  templateUrl: 'theme-toggle.component.html',
})
export class ThemeToggleComponent {
  private themeService = inject(ThemeService);

  isDarkMode = computed(
    () => this.themeService.getCurrentTheme() === 'business',
  );

  onThemeToggle(): void {
    this.themeService.toggleTheme();
  }
}
