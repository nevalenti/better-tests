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
  selector: 'app-register',
  templateUrl: 'register.component.html',
  imports: [
    RouterLink,
    ReactiveFormsModule,
    AuthCardComponent,
    OAuthButtonsComponent,
  ],
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  get firstName() {
    return this.form.get('firstName') as FormControl;
  }
  get lastName() {
    return this.form.get('lastName') as FormControl;
  }
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
