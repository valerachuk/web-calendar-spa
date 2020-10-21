import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(
    private router: Router,
    private authServece: AuthService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (route.data.onlyAuthorized) {
      if (this.authServece.isAuthenticated) {
        return true;
      } else {
        this.router.navigate(['auth']);
        return false;
      }
    }

    if (route.data.onlyAnonymous) {
      if (!this.authServece.isAuthenticated) {
        return true;
      } else {
        this.router.navigate(['']);
        return false;
      }
    }
    
  }
}
