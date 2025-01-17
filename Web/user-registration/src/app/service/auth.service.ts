import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from './app-config.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private configService: AppConfigService
  ) {
    this.apiUrl = this.configService.apiUrl;
  }

  // Register User
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/user/register`, user);
  }

  // Login User
  login(credentials: { username: string; password: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/user/authenticate`, credentials);
  }

  // Store JWT token after login
  storeToken(token: string): void {
    localStorage.setItem('access_token', token);
  }

  // Get JWT token
  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  // Check if the user is logged in
  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // Logout User
  logout(): void {
    localStorage.removeItem('access_token');
  }
}
