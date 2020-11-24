import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserInfo } from 'src/app/interfaces/user-info.interface';
import { CalendarEvent } from 'src/app/interfaces/event.interface';
import { CalendarEventService } from 'src/app/services/calendar-event.service';
import { CalendarService } from 'src/app/services/calendar.service';

@Component({
  selector: 'app-calendar-event-view',
  templateUrl: './calendar-event-view.component.html',
  styleUrls: ['./calendar-event-view.component.css']
})
export class CalendarEventViewComponent implements OnInit {

  eventId: number = null;
  calendarEvent: CalendarEvent = null;
  startDateTime: string = null;
  endDateTime: string = null;
  calendar: string = null;
  users: UserInfo[] = null;

  constructor
  (
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private eventService: CalendarEventService
  ) { }

  ngOnInit(): void {
  }

  getCalendarEvent(id: number) {
    this.eventService
    .getEvent(id)
    .subscribe(data => {
      this.calendarEvent = data;
      this.startDateTime = new Date(data.startDateTime).toLocaleString();
      this.endDateTime = new Date(data.endDateTime).toLocaleString();
      this.users = data.guests;
      this.getCalendar();
    });
  }

  getCalendar() {
    if(this.calendarEvent !== null){
    this.calendarService
    .getCalendar(this.calendarEvent.calendarId)
    .subscribe(data => {
      this.calendar = data.name;
    });
    }
  }

  export(){
    // TO DO
  }
}


