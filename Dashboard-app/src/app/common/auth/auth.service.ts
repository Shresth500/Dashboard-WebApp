import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ILogin, ILoginByUserName, IRegister } from '../Models/IAuth';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5121/api/auth'; // Update with your backend API URL
  private currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<any>(
      JSON.parse(localStorage.getItem('user') || 'null')
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  login(data: ILogin) {
    return this.http.post<any>(`${this.apiUrl}/login`, data).pipe(
      map((user) => {
        if (user && user['authToken']) {
          localStorage.setItem('user', JSON.stringify(user));
          localStorage.setItem('token', user['authToken']);
          this.currentUserSubject.next(user);
        }
        return user;
      })
    );
  }

  loginByUser(data: ILoginByUserName) {
    return this.http
      .post<any>(
        `${this.apiUrl}/LoginByUsername
`,
        data
      )
      .pipe(
        map((user) => {
          if (user && user['authToken']) {
            localStorage.setItem('user', JSON.stringify(user));
            localStorage.setItem('token', user['authToken']);
            this.currentUserSubject.next(user);
          }
          return user;
        })
      );
  }

  register(userData: IRegister) {
    return this.http.post<any>(`${this.apiUrl}/register`, userData, {
      responseType: 'text' as 'json', // 'json' is required here for type compatibility
    });
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken(); // Returns true if token exists
  }
}
