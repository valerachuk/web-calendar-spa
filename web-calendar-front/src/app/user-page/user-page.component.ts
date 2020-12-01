import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserInfo } from '../interfaces/user-info.interface';
import { saveAs } from 'file-saver';
import { ToastGlobalService } from '../services/toast-global.service';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { CalendarExportRange } from '../interfaces/calendarExportRange.interface';

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.css']
})
export class UserPageComponent implements OnInit {
  constructor(
    public authService: AuthService,
    private calendarService: CalendarService,
    private toastService: ToastGlobalService
  ) { }
  public userForm = new FormGroup({
    firstName: new FormControl(null, [Validators.required, this.noWhitespaceValidator]),
    lastName: new FormControl(null,  [Validators.required, this.noWhitespaceValidator]),
    notifications: new FormControl(null)
  });

  public startDate: NgbDateStruct;
  public endDate: NgbDateStruct;

  public useStartDate = false;
  public useEndDate = false;
  
  firstName = this.userForm.get('firstName');
  lastName = this.userForm.get('lastName');
  notifications = this.userForm.get('notifications');
  public email = this.authService.userEmail;

  public selectedCalendars: Calendar[] = [];
  public calendars: Calendar[];

  public errors = [];
  
  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
    this.resetForm();

    const date = new Date();
    this.startDate = { day: date.getDate(), month: date.getMonth() + 1, year: date.getFullYear() };
    this.endDate = { ...this.startDate };

  }

  public noWhitespaceValidator(control: FormControl) {
    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace;
    return isValid ? null : { 'whitespace': true };
  }

  resetForm() {
    this.firstName.setValue(this.authService.firstName);
    this.lastName.setValue(this.authService.lastName);
    this.notifications.setValue(this.authService.userNotify);
  }

  nothingChanged(): boolean {
    let firstNameEq = this.authService.firstName === this.firstName.value;
    let lastNameEq = this.authService.lastName === this.lastName.value;
    let notifyEq = this.authService.userNotify === this.notifications.value;
    return firstNameEq && lastNameEq && notifyEq;
  }

  saveChanges() {
    if(this.nothingChanged())
      return;
    if (!this.userForm.valid) {
      this.userForm.markAllAsTouched();
      return;
    }
    this.userForm.disable();
    let userInfo: UserInfo = {
      id: this.authService.userId,
      firstName: this.firstName.value,
      lastName: this.lastName.value,
      email: this.authService.userEmail,
      receiveEmailNotifications: this.notifications.value
    }
    this.authService.editUser(userInfo).subscribe(data => {
      this.toastService.add({
        delay: 5000,
        title: 'Success!',
        content: 'User info was updated successfully',
        className: "bg-success text-light"
      });
    }, err => {
      if(err.status === 403)
        this.errors.push(err.error.error);
      else
        this.errors.push(err.error.title);

      setTimeout(() => this.errors = [], 3000);
    })
    .add(()=> {
      this.resetForm();
      this.userForm.enable();
    });
  }

  private ngbDateToJsonDate(date: NgbDateStruct): string {
    console.log(date);
    return new Date(date.year, date.month - 1, date.day, 0, - (new Date()).getTimezoneOffset()).toJSON();
  }

  exportCalendar(): void {
    const calendarRange: CalendarExportRange = {};

    if (this.useStartDate) {
      calendarRange.from = this.ngbDateToJsonDate(this.startDate);
    }

    if (this.useEndDate) {
      calendarRange.to = this.ngbDateToJsonDate(this.endDate);
    }

    this.selectedCalendars
      .map(calendar => calendar.id)
      .forEach(id => this.calendarService.downloadCalendarIcs(id, calendarRange)
      .subscribe(response => {
        const uriEncodedFileName = response.headers.get('content-disposition').match(/filename\*=UTF-8''(.*?)$/)[1];
        const fileName = decodeURI(uriEncodedFileName);
        const file = new File([response.body], fileName, { type: response.body.type });
        saveAs(file);
      }));
  }
}
