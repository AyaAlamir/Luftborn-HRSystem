import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { interval, take } from 'rxjs';

interface AuthResponse {
  token?: string;
  accessToken?: string;
  access_token?: string;
  userName?: string;
  username?: string;
  name?: string;
  data?: AuthResponse;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));
  readonly isAuthenticated = signal(this.isBrowser && !!localStorage.getItem('access_token'));
  readonly userName = signal(this.isBrowser ? localStorage.getItem('user_name') ?? '' : '');
  private _googleInitialized = false;

  login(userName: string, password: string): Observable<void> {
    return this.http.post<AuthResponse>(`${API_BASE_URL}/Auth/login`, { userName, password }).pipe(
      tap(response => this.storeSession(response, userName)),
      map(() => undefined)
    );
  }

  loginWithGoogle(idToken: string): Observable<void> {
    return this.http.post<AuthResponse>(`${API_BASE_URL}/Auth/google`, { idToken }).pipe(
      tap(response => this.storeSession(response, 'Google User')),
      map(() => undefined)
    );
  }

  // Initialize Google Identity Services with the provided client ID.
  // Call this on app startup (e.g. login page) with your Google Client ID.
  initGoogle(clientId: string): void {
    if (!this.isBrowser) return;
    const win = window as any;

    const tryInit = () => {
      if (!win.google?.accounts?.id) return false;
      if (this._googleInitialized) return true;

      win.google.accounts.id.initialize({
        client_id: clientId,
        callback: (response: any) => {
          const idToken = response?.credential;
          if (!idToken) return;
          // Exchange token with backend and navigate on success
          this.loginWithGoogle(idToken).subscribe({
            next: () => this.router.navigate(['/employees']),
            error: () => {
              // ignore; loginWithGoogle will communicate errors via API if needed
            }
          });
        }
      });

      this._googleInitialized = true;
      return true;
    };

    if (!tryInit()) {
      // script may load after app bootstrap; poll briefly until available
      const sub = interval(300).pipe(take(10)).subscribe(() => {
        if (tryInit()) sub.unsubscribe();
      });
    }
  }

  // Trigger the Google One Tap / prompt flow. Requires initGoogle to have been called.
  requestGoogleSignIn(): void {
    if (!this.isBrowser) return;
    const win = window as any;
    if (!win.google?.accounts?.id) {
      console.warn('Google Identity Services not loaded.');
      return;
    }
    win.google.accounts.id.prompt();
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem('access_token');
      localStorage.removeItem('user_name');
    }
    this.isAuthenticated.set(false);
    this.userName.set('');
    this.router.navigate(['/login']);
  }

  private storeSession(response: AuthResponse, fallbackName: string): void {
    const body = response.data ?? response;
    const token = body.accessToken ?? body.access_token ?? body.token;
    if (!token) throw new Error('The API login response did not contain an access token.');

    const name = body.userName ?? body.username ?? body.name ?? fallbackName;
    if (this.isBrowser) {
      localStorage.setItem('access_token', token);
      localStorage.setItem('user_name', name);
    }
    this.userName.set(name);
    this.isAuthenticated.set(true);
  }
}
