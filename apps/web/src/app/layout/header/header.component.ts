import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle.component';

@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  imports: [RouterLink, ThemeToggleComponent],
})
export class HeaderComponent {}
