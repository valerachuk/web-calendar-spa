import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateStruct, NgbModal, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { CalendarEvent } from 'src/app/interfaces/event.interface';
import { UserInfo } from 'src/app/interfaces/user-info.interface';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarEventService } from 'src/app/services/calendar-event.service';
import { CalendarService } from 'src/app/services/calendar.service';
import { ToastGlobalService } from 'src/app/services/toast-global.service';
import { FileAttachService } from 'src/app/services/file-attach.service';
import { EditEventModalComponent } from '../edit-event-modal/edit-event-modal.component';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})

export class EventFormComponent implements OnInit {
  public error: string;

  startDate: NgbDateStruct;
  endDate: NgbDateStruct;
  startTime: NgbTimeStruct;
  endTime: NgbTimeStruct;
  attachedFile: File = null;

  calendars: Calendar[];
  calendarEvent: CalendarEvent;
  title = "Create new event";
  isRepeatable = false;

  users: UserInfo[] = null;
  selectedUsers: UserInfo[] = null;

  eventForm = new FormGroup({
    eventName: new FormControl(null, [Validators.required, Validators.maxLength(100), this.noWhitespaceValidator]),
    eventVenue: new FormControl(null, Validators.maxLength(100)),
    eventStartDate: new FormControl(null, Validators.required),
    eventStartTime: new FormControl(null, Validators.required),
    eventEndDate: new FormControl(null, Validators.required),
    eventEndTime: new FormControl(null, Validators.required),
    eventNotificationTime: new FormControl(null),
    eventCalendar: new FormControl(null, Validators.required),
    eventReiteration: new FormControl(null),
    eventGuests: new FormControl(null)
  });

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private authService: AuthService,
    private eventService: CalendarEventService,
    private modalService: NgbModal,
    private fileService: FileAttachService,
    private toastService: ToastGlobalService
  ) {
  }

  public noWhitespaceValidator(control: FormControl) {
    const targetStr = control.value || '';
    const isWhitespace = !!targetStr && !targetStr.trim();
    return isWhitespace ? { whitespace: true } : null;
  }

  ngOnInit() {
    this.dateTimeInit();
    this.calendarEventInit();
    this.usersInit();
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
      this.selectedUsers = data.guests;
    });
  }

  calendarEventInit() {
    this.calendarEvent = {} as CalendarEvent;
    this.calendarEvent.notificationTime = null;
    this.calendarEvent.reiteration = null;
    this.calendarEvent.venue = null;
    this.calendars = [];
    this.error = null;
    this.calendarService.get(this.authService.userId)
    .subscribe(data => {
      this.calendars = data;
    });
  }

  usersInit() {
    this.authService
    .getUsersExceptCurrent()
    .subscribe(data => this.users = data);
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
      time.minute 
      - (new Date()).getTimezoneOffset()
      ).toJSON();
  }

  submitEvent() {
    if (!this.eventForm.valid) {
      this.eventForm.markAllAsTouched();
      return;
    }
    if(this.attachedFile !== null && this.attachedFile !== undefined) {
      this.fileService.uploadFile(this.attachedFile).subscribe(data => {
        this.calendarEvent.fileId = data;
        this.eventRequest();
        this.toastService.add({
          delay: 1500,
          title: 'Success!',
          content: `File has been successfully uploaded`,
          className: "bg-success text-light"
        });
      }, err => {
        this.toastService.add({
          delay: 1500,
          title: 'Error!',
          content: `Upload file error: ${err.message}`,
          className: "bg-danger text-light"
        });
      });
    }
    else
      this.eventRequest();
  }

  eventRequest() {
    let start = this.calendarEventDateTimeAssembly(this.startTime, this.startDate);
    let end = this.calendarEventDateTimeAssembly(this.endTime, this.endDate);

    

    let isSameDate = moment.utc(start).isSame(moment.utc(this.calendarEvent.startDateTime))
      && moment.utc(end).isSame(moment.utc(this.calendarEvent.endDateTime));

    console.log(moment.utc(start));
    console.log(moment.utc(this.calendarEvent.startDateTime));

    this.calendarEvent.startDateTime = start;
    this.calendarEvent.endDateTime = end;

    this.calendarEvent.guests = this.selectedUsers;

    this.eventForm.disable();
    let httpMethod;

    if (!this.calendarEvent.id) {
      httpMethod = this.eventService.addEvent(this.calendarEvent);
      this.httpRequest(httpMethod);
    } else {
      // dates have impact on event series, so if user change event date - edit only this event 
      if (this.isRepeatable && isSameDate) {
        let modalRef = this.modalService.open(EditEventModalComponent, { centered: true, size: 'sm' });
        modalRef.result.then(
          isSeriesEdit => {
            httpMethod = isSeriesEdit ?
              this.eventService.updateEventSeries(this.calendarEvent) :
              this.eventService.updateEvent(this.calendarEvent);
            this.httpRequest(httpMethod);
          },
          ()=> this.eventForm.enable());
      } else {
        httpMethod = this.eventService.updateEvent(this.calendarEvent);
        this.httpRequest(httpMethod);
      }
    }
  }

  httpRequest(httpMethod) {
    httpMethod
      .subscribe(response => {
        setTimeout(() => { this.activeModal.close() }, 1000);
        this.toastService.add({
          delay: 5000,
          title: 'Success!',
          content: 'Event was saved successfully',
          className: "bg-success text-light"
        });
      },
        error => {
          this.error = error.error;
          this.eventForm.enable();
        }, () => {
          this.error = null;
          this.dateTimeInit();
          this.eventForm.enable();
        });

    this.error = null;
  }
}