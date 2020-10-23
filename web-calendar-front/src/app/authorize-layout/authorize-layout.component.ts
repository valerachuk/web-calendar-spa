import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-authorize-layout',
  templateUrl: './authorize-layout.component.html',
  styleUrls: ['./authorize-layout.component.css']
})
export class AuthorizeLayoutComponent {

  constructor(
    private authService: AuthService
  ) { }

  public signOut(): void {
    this.authService.signOut();
  }

}
