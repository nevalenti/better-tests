import { Component, input } from '@angular/core';

@Component({
  selector: 'app-auth-card',
  templateUrl: 'auth-card.component.html',
})
export class AuthCardComponent {
  subtitle = input.required<string>();
}
