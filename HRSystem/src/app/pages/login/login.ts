import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({ selector: 'app-login', imports: [FormsModule], templateUrl: './login.html', styleUrl: './login.css' })
export class Login {
  username = '';
  password = '';
  error = '';
  showPassword = false;
  loading = false;

  constructor(private auth: AuthService, private router: Router) {
    if (auth.isAuthenticated()) router.navigate(['/employees']);
  }

  ngOnInit(): void {
    // Initialize Google Identity Services with a placeholder client id.
    // Replace 'YOUR_GOOGLE_CLIENT_ID' with your real client id from Google Cloud Console.
    this.auth.initGoogle('YOUR_GOOGLE_CLIENT_ID');
  }

  submit(): void {
    if (!this.username.trim() || !this.password) {
      this.error = 'Please enter your username and password.';
      return;
    }

    this.error = '';
    this.loading = true;
    this.auth.login(this.username.trim(), this.password).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: error => {
        this.loading = false;
        this.error = this.apiError(error, 'Login failed. Check your username and password.');
      }
    });
  }

  ssoLogin(): void {
    // Trigger the Google sign-in prompt. The credential response will be handled
    // by the callback passed when initGoogle() was called.
    this.auth.requestGoogleSignIn();
  }

  private apiError(error: unknown, fallback: string): string {
    if (!(error instanceof HttpErrorResponse)) return fallback;
    return error.error?.message ?? error.error?.title ?? (typeof error.error === 'string' ? error.error : fallback);
  }
}
