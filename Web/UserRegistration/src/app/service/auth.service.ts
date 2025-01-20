import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = environment.ApiUrl;
  }

  // Register User
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/account/register`, user);
  }

  // Login User
  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/account/login`, credentials);
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

  // Get list of users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/api/account/GetAllUsers`);
  }

  // Check if the user has admin role
  isAdmin(): boolean {
    const token = this.getToken();
    if (token) {
      const decodedToken: any = jwtDecode(token);
      return decodedToken.role && decodedToken.role.toLowerCase() === 'admin';
    }
    return false;
  }
}

// User model interface
export interface User {
  firstName: string;
  lastName: string;
  email: string;
}
