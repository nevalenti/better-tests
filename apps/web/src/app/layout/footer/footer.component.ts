import { Component } from '@angular/core';

import { ThemeToggleComponent } from '../../shared/theme-toggle/theme-toggle.component';

@Component({
  selector: 'app-footer',
  templateUrl: 'footer.component.html',
  imports: [ThemeToggleComponent],
})
export class FooterComponent {}
