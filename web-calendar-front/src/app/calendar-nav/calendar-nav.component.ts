import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CalendarService } from '../services/calendar.service';
import { Calendar } from '../interfaces/calendar.interface';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})
export class CalendarNavComponent implements OnInit {
  public calendars: Calendar[];
  public userName = localStorage.getItem('firstName');

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
    private calendarService: CalendarService
  ) { }

  ngOnInit(): void {
    this.calendarService.get(Number(localStorage.getItem('userId'))).subscribe(data => {
      this.calendars = data;
    });
  }

  openModal(content) {
   this.modalService.open(content, { centered: true });
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
      userId: Number(localStorage.getItem('userId'))
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
