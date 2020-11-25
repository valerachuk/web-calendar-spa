import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SignInFormComponent } from './sign-in-form/sign-in-form.component';
import { SignUpFormComponent } from './sign-up-form/sign-up-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { ACCESS_TOKEN_KEY } from './services/auth.service';
import { DefaultLayoutComponent } from './default-layout/default-layout.component';
import { AuthorizeLayoutComponent } from './authorize-layout/authorize-layout.component';
import { MyIdComponent } from './my-id/my-id.component';
import { ToastGlobalComponent } from './toast-global/toast-global.component';
import { UserPageComponent } from './user-page/user-page.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ErrorHandlerModule } from './global-error-handler/error-handler.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

function tokenGetter(): string {
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

@NgModule({
  declarations: [
    AppComponent,
    SignInFormComponent,
    SignUpFormComponent,
    DefaultLayoutComponent,
    AuthorizeLayoutComponent,
    MyIdComponent,
    ToastGlobalComponent,
    UserPageComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    NgSelectModule,
    ErrorHandlerModule,
    BrowserAnimationsModule,
    FontAwesomeModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        allowedDomains: environment.allowedApiDomainsAuth
      }
    }),
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
