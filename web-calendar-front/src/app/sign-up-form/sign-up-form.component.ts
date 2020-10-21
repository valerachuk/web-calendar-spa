import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-sign-up-form',
  templateUrl: './sign-up-form.component.html',
  styleUrls: ['./sign-up-form.component.css']
})
export class SignUpFormComponent {

  public form = new FormGroup({
    firstName: new FormControl(null, Validators.required),
    lastName: new FormControl(null, Validators.required),
    email: new FormControl(null, [Validators.required, Validators.email]),
    password: new FormControl(null, Validators.required)
  });

  public isEmailExsists = false;

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

    this.authService.signUp(this.form.value)
      .subscribe(
        (token) => {
          console.log('Logged in, all fine');
          console.log(token);
          this.router.navigate(['']);
        },
        error => {
          console.log('Token error');
          console.log(error);
          if (error.status === 409) {
            this.isEmailExsists = true;
            this.form.enable();
          }
        }
      );
  }

}
