import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Login } from '../interfaces/login.interface';
import { Register } from '../interfaces/register.interface';
import { SignInfo } from '../interfaces/signInfo.interface';

export const ACCESS_TOKEN_KEY = 'access_token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl;
  public userId: number;
  public firstName: string;
  
  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private jwtHelper: JwtHelperService
  ) { 
    this.userId = (localStorage.getItem('userId') !== null) ? Number(localStorage.getItem('userId')) : undefined; 
    this.firstName = (localStorage.getItem('firstName') !== null) ? localStorage.getItem('firstName') : undefined; 
  }

  private initStorageInfo(info) {
    localStorage.setItem(ACCESS_TOKEN_KEY, info.access_token);
    localStorage.setItem('userId', info.userId.toString());
    localStorage.setItem('firstName', info.firstName);
    this.userId = info.userId;
    this.firstName = info.firstName;
  }

  public signIn(login: Login): Observable<SignInfo> {
    return this.httpClient.post<SignInfo>(`${this.apiUrl}/auth/sign-in`, login)
    .pipe(
      tap(info => {
        this.initStorageInfo(info);
      })
    );
  }

  public signUp(register: Register): Observable<SignInfo> {
    return this.httpClient.post<SignInfo>(`${this.apiUrl}/auth/sign-up`, register)
    .pipe(
      tap(info => {
        this.initStorageInfo(info);
      })
    );
  }

  public get isAuthenticated(): boolean {
    const token = localStorage.getItem(ACCESS_TOKEN_KEY);
    return token && !this.jwtHelper.isTokenExpired(token);
  }

  public signOut(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem('userId');
    localStorage.removeItem('firstName');
    this.router.navigate(['auth']);
  }

}
