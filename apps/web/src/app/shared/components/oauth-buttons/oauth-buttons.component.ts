import { Component, input } from '@angular/core';

@Component({
  selector: 'app-oauth-buttons',
  templateUrl: 'oauth-buttons.component.html',
})
export class OAuthButtonsComponent {
  action = input<string>('Log in');
}
