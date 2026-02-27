import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AuthCardComponent } from '../../shared/components/auth-card/auth-card.component';
import { OAuthButtonsComponent } from '../../shared/components/oauth-buttons/oauth-buttons.component';

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html',
  imports: [
    RouterLink,
    ReactiveFormsModule,
    AuthCardComponent,
    OAuthButtonsComponent,
  ],
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  get email() {
    return this.form.get('email') as FormControl;
  }
  get password() {
    return this.form.get('password') as FormControl;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
  }
}
