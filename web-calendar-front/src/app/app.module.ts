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
import { CalendarComponent } from './calendar/calendar.component';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/moment';
import * as moment from 'moment';
import { ToastGlobalComponent } from './toast-global/toast-global.component';
import { CalendarNavComponent } from './calendar-nav/calendar-nav.component';
import { EventFormComponent } from './event-form/event-form.component';

export function momentAdapterFactory() {
  return adapterFactory(moment);
};

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
    CalendarComponent,
    ToastGlobalComponent,
    CalendarNavComponent,
    EventFormComponent
  ],
  exports: [EventFormComponent],
  entryComponents: [EventFormComponent],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    JwtModule.forRoot({
      config: {
        tokenGetter,
        allowedDomains: environment.allowedApiDomainsAuth
      }
    }),
    CalendarModule.forRoot({ provide: DateAdapter, useFactory: momentAdapterFactory })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
