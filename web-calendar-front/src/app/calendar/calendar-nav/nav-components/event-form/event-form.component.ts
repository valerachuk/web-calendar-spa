import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateStruct, NgbModal, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';

import { Calendar } from 'src/app/interfaces/calendar.interface';
import { NotificationTime, Reiteration, CalendarEvent } from 'src/app/interfaces/event.interface';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarEventService } from 'src/app/services/calendar-event.service';
import { CalendarService } from 'src/app/services/calendar.service';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})

export class EventFormComponent implements OnInit {

  public addedNewCalendarEvent = false;
  public error: string;

  startDate: NgbDateStruct;
  endDate: NgbDateStruct;
  startTime: NgbTimeStruct;
  endTime: NgbTimeStruct;

  calendars: Calendar[];
  calendarEvent: CalendarEvent;

  eventForm = new FormGroup({
    eventName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    eventVenue: new FormControl(null, Validators.maxLength(100)),
    eventStartDate: new FormControl(null, Validators.required),
    eventStartTime: new FormControl(null, Validators.required),
    eventEndDate: new FormControl(null, Validators.required),
    eventEndTime: new FormControl(null, Validators.required),
    eventNotificationTime: new FormControl(null),
    eventCalendar: new FormControl(null, Validators.required),
    eventReiteration: new FormControl(null)
  });

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private authService: AuthService,
    private eventService: CalendarEventService
  ) { }

  ngOnInit() {
    this.dateTimeInit();
    this.calendarEventInit();
  }

  calendarEventInit() {
    this.calendarEvent = {} as CalendarEvent;
    this.calendarEvent.notificationTime = null;
    this.calendarEvent.reiteration = null;
    this.calendarEvent.venue = null;
    this.calendars = [];
    this.error = null;
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
  }

  dateTimeInit() {
    var currentDate = moment.utc();
    var currentTime = new Date();

    this.startDate = { day: currentDate.date(), month: currentDate.month() + 1, year: currentDate.year() };
    this.endDate = { day: currentDate.date(), month: currentDate.month() + 1, year: currentDate.year() };
    this.startTime = { hour: currentTime.getHours(), minute: currentTime.getMinutes(), second: currentTime.getSeconds() };
    this.endTime = { hour: currentTime.getHours() + 1, minute: currentTime.getMinutes(), second: currentTime.getSeconds() };
  }

  calendarEventDateTimeAssembly(time: NgbTimeStruct, date: NgbDateStruct): string {
    return new Date(
      date.year,
      date.month - 1,
      date.day,
      time.hour,
      time.minute - (new Date()).getTimezoneOffset()).toJSON();
  }

  submitEvent() {
    if (!this.eventForm.valid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    this.calendarEvent.startDateTime = this.calendarEventDateTimeAssembly(this.startTime, this.startDate);
    this.calendarEvent.endDateTime = this.calendarEventDateTimeAssembly(this.endTime, this.endDate);
    this.eventForm.disable();

    this.eventService.addEvent(this.calendarEvent)
      .subscribe(response => {
        this.addedNewCalendarEvent = true;
        setTimeout(() => this.addedNewCalendarEvent = false, 2500);
      },
        error => {
          this.error = error.error;
          this.eventForm.enable();
        }, () => {
          this.error = null;
          this.eventForm.reset();
          this.dateTimeInit();
          this.eventForm.enable();
        });
    this.error = null;
  }
}