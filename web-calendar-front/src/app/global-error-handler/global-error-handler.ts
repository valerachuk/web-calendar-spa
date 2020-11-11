import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, Injectable } from '@angular/core';
import { ToastGlobalService } from '../services/toast-global.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

  constructor(
    private toastService: ToastGlobalService
  ) { }

  handleError(err: Error | HttpErrorResponse) {
    let message = err.message;
    if(err instanceof HttpErrorResponse)
      message = err.error.error ? err.error.error : err.message;

    this.toastService.add({
      delay: 5000,
      title: err instanceof HttpErrorResponse ? `Error ${err.status} from server` : 'Client error message',
      content: message,
      className: "bg-danger text-light"
    });
  }
}
