import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserInfo } from '../interfaces/user-info.interface';

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
    lastName: new FormControl(null,  Validators.required)
  });
  firstName = this.userForm.get('firstName');
  lastName = this.userForm.get('lastName');
  public email = this.authService.userEmail;

  public selectedCalendars: Calendar[] = [];
  public calendars: Calendar[];

  public savedChanges = false;
  
  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
    this.firstName.setValue(this.authService.firstName);
    this.lastName.setValue(this.authService.lastName);
  }

  saveChanges() {
    if(this.authService.firstName === this.firstName.value && this.authService.lastName === this.lastName.value)
      return;
    let userInfo: UserInfo = {
      id: this.authService.userId,
      firstName: this.firstName.value,
      lastName: this.lastName.value,
      email: this.authService.userEmail
    }
    this.authService.editUser(userInfo).subscribe(data => {
      this.savedChanges = true;
      setTimeout(()=> this.savedChanges = false, 1000);
    });
  }

  exportCalendar() {
    console.log(this.selectedCalendars);
  }
}
