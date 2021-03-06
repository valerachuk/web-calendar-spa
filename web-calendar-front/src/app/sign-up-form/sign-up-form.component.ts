import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { faEnvelope, faLock, faUser } from '@fortawesome/free-solid-svg-icons';
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
    password: new FormControl(null, [Validators.required, Validators.pattern("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$")])
  });

  faEnvelope = faEnvelope;
  faLock = faLock;
  faUser = faUser;
  
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
          this.router.navigate(['']);
        },
        error => {
          if (error.status === 409)
            this.isEmailExsists = true;
          else if(error.status === 400 && error.error.errors.Password)
            this.form.get('password').setErrors({ pattern: true });
        }
      ).add(
        this.form.enable()
      );
  }
}
