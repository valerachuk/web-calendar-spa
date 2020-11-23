import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserInfo } from '../interfaces/user-info.interface';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.css']
})
export class UserPageComponent implements OnInit {
  constructor(
    public authService: AuthService,
    private calendarService: CalendarService
  ) { }
  public userForm = new FormGroup({
    firstName: new FormControl(null, Validators.required),
    lastName: new FormControl(null,  Validators.required),
    notifications: new FormControl(null)
  });
  firstName = this.userForm.get('firstName');
  lastName = this.userForm.get('lastName');
  notifications = this.userForm.get('notifications');
  public email = this.authService.userEmail;

  public selectedCalendars: Calendar[] = [];
  public calendars: Calendar[];

  public savedChanges = false;
  public errors = [];
  
  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
    this.resetForm();
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
      this.savedChanges = true;
      setTimeout(()=> this.savedChanges = false, 2000);
    }, err => {
      if(err.status === 403)
        this.errors.push(err.error.error);
      else
        this.errors.push(err.error.title);

      setTimeout(() => this.errors = [], 2000);
    })
    .add(()=> {
      this.resetForm();
      this.userForm.enable();
    });
  }

  exportCalendar(): void {
    this.selectedCalendars
      .map(calendar => calendar.id)
      .forEach(id => this.calendarService.downloadCalendarIcs(id).subscribe(response => {
        const uriEncodedFileName = response.headers.get('content-disposition').match(/filename\*=UTF-8''(.*?)$/)[1];
        const fileName = decodeURI(uriEncodedFileName);
        const file = new File([response.body], fileName, { type: response.body.type });
        saveAs(file);
      }));
  }
}
