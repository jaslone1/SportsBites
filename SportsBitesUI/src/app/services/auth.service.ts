import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient, private router: Router) {
  }

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(res => {
        console.log('Login Response:', res);
        if (res.token) {
          localStorage.setItem('game_day_token', res.token);
        }
        if (res.username) {
          localStorage.setItem('username', res.username);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('game_day_token');
    localStorage.removeItem('username');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('game_day_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      // Check the standard JWT claim locations for the username
      return payload.unique_name ||
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
        localStorage.getItem('username'); // Fallback to your old method just in case
    } catch (e) {
      console.error("Error decoding token for username", e);
      return null;
    }
  }

  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      // A JWT token has 3 parts separated by dots. The 2nd part [1] is the data.
      // atob decodes the Base64 string so we can read the JSON inside.
      const payload = JSON.parse(atob(token.split('.')[1]));

      // .NET usually stores the ID in one of these two keys:
      return payload.nameid ||
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
        null;
    } catch (e) {
      console.error("Error decoding token", e);
      return null;
    }
  }
}
