import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Login } from '../interfaces/login.interface';
import { Register } from '../interfaces/register.interface';
import { Token } from '../interfaces/token.interface';
import { UserInfo } from '../interfaces/user-info.interface';

export const ACCESS_TOKEN_KEY = 'access_token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl;
  public userId: number;
  public firstName: string;
  public lastName: string;
  public userEmail: string;
  public userNotify: boolean;
  
  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private jwtHelper: JwtHelperService
  ) { 
    if(this.userId === undefined)
      this.decodeUserToken();
  }

  private decodeUserToken() {
    try {
      let token = localStorage.getItem(ACCESS_TOKEN_KEY);
      let tokenInfo = this.jwtHelper.decodeToken(token);
      this.userId = Number(tokenInfo.sub);
      this.firstName = tokenInfo.firstName;
      this.lastName = tokenInfo.lastName;
      this.userEmail = tokenInfo.email;
      this.userNotify = (tokenInfo.notifications === 'True');
    }
    catch {
      this.router.navigate(['auth']);
    }
  }

  private initStorageInfo(info) {
    localStorage.setItem(ACCESS_TOKEN_KEY, info.access_token);
  }

  public signIn(login: Login): Observable<Token> {
    return this.httpClient.post<Token>(`${this.apiUrl}/auth/sign-in`, login)
    .pipe(
      tap(info => {
        this.initStorageInfo(info);
        this.decodeUserToken();
      })
    );
  }

  public signUp(register: Register): Observable<Token> {
    return this.httpClient.post<Token>(`${this.apiUrl}/auth/sign-up`, register)
    .pipe(
      tap(info => {
        this.initStorageInfo(info);
        this.decodeUserToken();
      })
    );
  }

  public editUser(userInfo: UserInfo): Observable<Token> {
    return this.httpClient.put<Token>(`${this.apiUrl}/auth/edit`, userInfo)
    .pipe(
      tap(info => {
        this.initStorageInfo(info);
        this.decodeUserToken();
      })
    );
  }

  public getUsersExceptCurrent(): Observable<UserInfo[]> {
    return this.httpClient.get<UserInfo[]>(`${this.apiUrl}/auth/GetAllUsers`);
  }

  public get isAuthenticated(): boolean {
    const token = localStorage.getItem(ACCESS_TOKEN_KEY);
    return token && !this.jwtHelper.isTokenExpired(token);
  }

  public signOut(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    this.router.navigate(['auth']);
  }
}
