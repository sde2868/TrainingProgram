import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { API_CONFIG } from '../services/api-config';
import { LoginRequest, LoginResponse, LoggedInUser } from '../../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private readonly tokenKey = 'tm_auth_token';
  private readonly userKey = 'tm_auth_user';

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_CONFIG.baseUrl}/auth/login`, request)
      .pipe(
        tap((response) => {
          this.setSession(response);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getCurrentUser(): LoggedInUser | null {
    const userJson = localStorage.getItem(this.userKey);

    if (!userJson) {
      return null;
    }

    return JSON.parse(userJson) as LoggedInUser;
  }

  isLoggedIn(): boolean {
    return Boolean(this.getToken());
  }

  private setSession(response: LoginResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify(response.user));
  }
}