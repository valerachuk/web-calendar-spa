import { ErrorHandler, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GlobalErrorHandler } from './global-error-handler';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ServerErrorInterceptor } from './server-error.interceptor';
import { NotFoundPageComponent } from './error-pages/not-found-page/not-found-page.component';

@NgModule({
  declarations: [
    NotFoundPageComponent
  ],
  imports: [
    CommonModule
  ],
  providers: [
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    { provide: HTTP_INTERCEPTORS, useClass: ServerErrorInterceptor, multi: true }
  ]
})
export class ErrorHandlerModule { }
