import { Injectable } from '@angular/core';
import { Toast } from '../interfaces/toast.interface';

@Injectable({
  providedIn: 'root'
})
export class ToastGlobalService {

  public toasts: Toast[] = [];
  
  public add(toast: Toast): void {
    this.toasts.push(toast);
  }

  public delete(toast: Toast): void {
    this.toasts = this.toasts.filter(t => t !== toast);
  }

}
