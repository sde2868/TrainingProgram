import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { Auth } from '../../core/auth/auth';
import { LoginRequest } from '../../models/auth.models';
import { TextInput } from '../../shared/components/text-input/text-input';
import { Button } from '../../shared/components/button/button';
import { ErrorMessage } from '../../shared/components/error-message/error-message';

@Component({
  selector: 'app-login',
  imports: [FormsModule, TextInput, Button, ErrorMessage],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  form: LoginRequest = {
    username: '',
    password: ''
  };

  isLoading = false;
  errorMessage = '';

  constructor(
    private readonly auth: Auth,
    private readonly router: Router
  ) {}

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.form.username.trim() || !this.form.password) {
      this.errorMessage = 'Username and password are required.';
      return;
    }

    this.isLoading = true;

    this.auth.login({
      username: this.form.username.trim(),
      password: this.form.password
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigateByUrl('/dashboard');
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error?.error?.message || 'Login failed. Please check your credentials.';
      }
    });
  }
}