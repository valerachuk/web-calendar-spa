import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login-form',
  templateUrl: './sign-in-form.component.html',
  styleUrls: ['./sign-in-form.component.css']
})
export class SignInFormComponent {

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

    console.log(this.form.value);
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
