import { Component } from '@angular/core';
import { ToastGlobalService } from '../services/toast-global.service';

@Component({
  selector: 'app-toast-global',
  templateUrl: './toast-global.component.html',
  styleUrls: ['./toast-global.component.css']
})
export class ToastGlobalComponent {

  constructor(
    public toastService: ToastGlobalService
  ) { }

}
