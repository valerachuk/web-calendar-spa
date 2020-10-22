import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';
import { ToastGlobalService } from './toast-global.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(
    private router: Router,
    private authService: AuthService,
    private toastService: ToastGlobalService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (route.data.onlyAuthorized) {
      if (this.authService.isAuthenticated) {
        return true;
      } else {
        this.router.navigate(['auth']);
        this.toastService.add({
          delay: 5000,
          title: 'Info',
          content: 'Login again, please'
        });
        return false;
      }
    }

    if (route.data.onlyAnonymous) {
      if (!this.authService.isAuthenticated) {
        return true;
      } else {
        this.router.navigate(['']);
        this.toastService.add({
          delay: 5000,
          title: 'Info',
          content: 'You have already signed in'
        });
        return false;
      }
    }
    
  }
}
