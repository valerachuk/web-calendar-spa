import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeLayoutComponent } from './authorize-layout/authorize-layout.component';
import { DefaultLayoutComponent } from './default-layout/default-layout.component';
import { MyIdComponent } from './my-id/my-id.component';
import { AuthGuardService } from './services/auth-guard.service';
import { SignInFormComponent } from './sign-in-form/sign-in-form.component';
import { SignUpFormComponent } from './sign-up-form/sign-up-form.component';
import { UserPageComponent } from './user-page/user-page.component';
import { NotFoundPageComponent } from './global-error-handler/error-pages/not-found-page/not-found-page.component';
import { ServerErrorPageComponent } from './global-error-handler/error-pages/server-error-page/server-error-page.component';

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
        loadChildren: () => import('./calendar/web-calendar.module').then(module => module.WebCalendarModule)
      },
      {
        path: 'user',
        component: UserPageComponent
      },
      {
        path: '404',
        component: NotFoundPageComponent
      },
      { 
        path: '500',
        component: ServerErrorPageComponent
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
  },
  {
    path: '**',
    redirectTo: '404',
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
