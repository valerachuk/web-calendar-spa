import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeLayoutComponent } from './authorize-layout/authorize-layout.component';
import { DefaultLayoutComponent } from './default-layout/default-layout.component';
import { MyIdComponent } from './my-id/my-id.component';
import { AuthGuardService } from './services/auth-guard.service';
import { SignInFormComponent } from './sign-in-form/sign-in-form.component';
import { SignUpFormComponent } from './sign-up-form/sign-up-form.component';
import { CalendarComponent } from './calendar/calendar.component';

const routes: Routes = [
  {
    path: '',
    component: AuthorizeLayoutComponent,
    data: {
      onlyAuthorized: true
    },
    canActivate: [
      AuthGuardService
    ],
    children: [
      {
        path: '',
        redirectTo: 'calendar',
        pathMatch: 'full'
      },
      {
        path: 'id',
        component: MyIdComponent
      },
      {
        path: 'calendar',
        component: CalendarComponent
      }
    ]
  },
  {
    path: 'auth',
    component: DefaultLayoutComponent,
    data: {
      onlyAnonymous: true
    },
    canActivate: [
      AuthGuardService
    ],
    children: [
      {
        path: '',
        redirectTo: 'sign-in',
        pathMatch: 'full'
      },
      {
        path: 'sign-in',
        component: SignInFormComponent
      },
      {
        path: 'sign-up',
        component: SignUpFormComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
