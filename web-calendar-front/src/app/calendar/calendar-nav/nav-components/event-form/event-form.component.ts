import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateStruct, NgbModal, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { CalendarEvent } from 'src/app/interfaces/event.interface';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarEventService } from 'src/app/services/calendar-event.service';
import { CalendarService } from 'src/app/services/calendar.service';
import { EditEventModalComponent } from '../edit-event-modal/edit-event-modal.component';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})

export class EventFormComponent implements OnInit {

  public successfullySavedEvent = false;
  public error: string;

  startDate: NgbDateStruct;
  endDate: NgbDateStruct;
  startTime: NgbTimeStruct;
  endTime: NgbTimeStruct;

  calendars: Calendar[];
  calendarEvent: CalendarEvent;
  title = "Create new event";
  isRepeatable = false;

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
    private eventService: CalendarEventService,
    private modalService: NgbModal
  ) {
  }

  ngOnInit() {
    this.dateTimeInit();
    this.calendarEventInit();
  }

  getcalendarEvent(id: number) {
    this.title = "Edit the event";
    this.eventService.getEvent(id).subscribe(data => {
      this.calendarEvent = data;
      let date = new Date(data.startDateTime);
      this.startDate = { day: date.getDate(), month: date.getMonth() + 1, year: date.getFullYear() };
      this.startTime = { hour: date.getHours(), minute: date.getMinutes(), second: date.getSeconds() };
      date = new Date(data.endDateTime);
      this.endDate = { day: date.getDate(), month: date.getMonth() + 1, year: date.getFullYear() };
      this.endTime = { hour: date.getHours(), minute: date.getMinutes(), second: date.getSeconds() };
      this.isRepeatable = data.reiteration ? true : false;
    });
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
    let currentDate = moment.utc();
    let currentTime = new Date();

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
    let start = this.calendarEventDateTimeAssembly(this.startTime, this.startDate);
    let end = this.calendarEventDateTimeAssembly(this.endTime, this.endDate);

    let isDateChanged =
      moment(start).isSame(this.calendarEvent.startDateTime)
      && moment(end).isSame(this.calendarEvent.endDateTime)

    this.calendarEvent.startDateTime = start;
    this.calendarEvent.endDateTime = end;

    this.eventForm.disable();
    let httpMethod;
    if (!this.calendarEvent.id) {
      httpMethod = this.eventService.addEvent(this.calendarEvent);
      this.httpRequest(httpMethod);
    } else {
      // dates have impact on event series, so if user change event date - edit only this event 
      if (this.isRepeatable && !isDateChanged) {
        let modalRef = this.modalService.open(EditEventModalComponent, { centered: true, size: 'sm' });
        modalRef.result.then(
          isSeriesEdit => {
            httpMethod = isSeriesEdit ?
              this.eventService.updateEventSeries(this.calendarEvent) :
              this.eventService.updateEvent(this.calendarEvent);
            this.httpRequest(httpMethod);
          });
      } else {
        httpMethod = this.eventService.updateEvent(this.calendarEvent);
        this.httpRequest(httpMethod);
      }
    }
  }

  httpRequest(httpMethod) {
    httpMethod
      .subscribe(response => {
        this.successfullySavedEvent = true;
        setTimeout(() => { this.successfullySavedEvent = false; this.activeModal.close() }, 1500);
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