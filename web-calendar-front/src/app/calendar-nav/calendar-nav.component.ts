import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CalendarService } from '../services/calendar.service';
import { Calendar } from '../interfaces/calendar.interface';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})
export class CalendarNavComponent implements OnInit {
  @ViewChild('delCalendar') deleteModal: any;
  isCalendarOwner: boolean;
  deleteCalendarId: number;

  public calendars: Calendar[];
  public userName = this.authService.firstName;

  public addCalendarForm = new FormGroup({
    newCalendarName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    newCalendarDesc: new FormControl(null, Validators.maxLength(1000))
  });
  public addedNewCalendar = false;
  public errors = [];
  calendarName = this.addCalendarForm.get('newCalendarName');
  calendarDesc = this.addCalendarForm.get('newCalendarDesc');

  constructor(
    private modalService: NgbModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
  }

  openModal(content, mdSize) {
    this.modalService.open(content, { centered: true, size: mdSize});
   }

  confirmDeleteCalendar(calendarId: number) {
    if(this.calendars.find(c => c.id === calendarId).userId !== this.authService.userId) {
      this.isCalendarOwner = false;
      this.openModal(this.deleteModal, 'sm');
      return;
    }
    this.isCalendarOwner = true;
    this.deleteCalendarId = calendarId;
    this.openModal(this.deleteModal, 'sm');
  }

  deleteCalendar(id: number) {
    this.calendarService.delete(id).subscribe(id => {
      let index = this.calendars.findIndex(calendar => calendar.id == id);
      this.calendars.splice(index, 1);
    });
  }

  addCalendar(){
    if (!this.addCalendarForm.valid) {
      this.addCalendarForm.markAllAsTouched();
      return;
    }
    this.addCalendarForm.disable();
    var newCalendar: Calendar = {
      id: 0,
      name: this.addCalendarForm.value.newCalendarName,
      description: this.addCalendarForm.value.newCalendarDesc,
      userId: this.authService.userId
    }
    this.calendarService.addCalendar(newCalendar).subscribe(calendar => {
      this.calendars.push(calendar);
      this.addedNewCalendar = true;
      setTimeout(()=> this.addedNewCalendar = false, 2500);
    }, err => {
      if(err.status == 400)
        this.errors.push("Error code 400, calendar not added");
    },() => { 
      this.addCalendarForm.reset();
      this.addCalendarForm.enable();
    });
  }
}
