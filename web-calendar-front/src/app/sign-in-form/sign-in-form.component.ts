import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { faEnvelope, faLock } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login-form',
  templateUrl: './sign-in-form.component.html',
  styleUrls: ['./sign-in-form.component.css']
})
export class SignInFormComponent {

  faEnvelope = faEnvelope;
  faLock = faLock;

  public form = new FormGroup({
    email: new FormControl(null, Validators.required),
    password: new FormControl(null, Validators.required)
  });

  public isInvalidCredentials = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  public submit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    this.form.disable();

    this.authService.signIn(this.form.value)
      .subscribe(
        () => this.router.navigate(['']),
        error => {
          if (error.status === 422) {
            this.isInvalidCredentials = true;
            this.form.enable();
          }
        }
      );
  }

}
