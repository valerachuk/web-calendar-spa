import { Component, OnInit } from '@angular/core';
import { CalendarItemsService } from '../services/calendar-items.service';
import {
  CalendarDayViewBeforeRenderEvent,
  CalendarEvent,
  CalendarMonthViewBeforeRenderEvent,
  CalendarView,
  CalendarWeekViewBeforeRenderEvent
} from 'angular-calendar';
import * as moment from 'moment';


@Component({
  selector: 'calendar-component',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})


export class CalendarComponent implements OnInit {

  Moment = moment.updateLocale('en', {
    week: {
      dow: 1,
    },
  });
  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;

  isActiveDayOpen = false;
  
  viewDate: Date = new Date();
  events: CalendarEvent[] = [];

  startDate: Date;
  endDate: Date;

  selectedCalendars = [];

  constructor(
    private calendarComponentService: CalendarItemsService) { }

  ngOnInit() {
    this.startDate = this.viewDate;
    this.endDate = this.viewDate;
  }

  setView(view: CalendarView) {
    this.view = view;
  }

  public updateCalendarItems() {
    if (this.selectedCalendars.length > 0) {
      this.calendarComponentService.getCalendarsItems(
        this.startDate.toJSON(), 
        this.endDate.toJSON(), 
        this.selectedCalendars)
        .subscribe(calendarItems => {
          calendarItems.map(item => {
            item.title = item['name'];
            item.start = new Date(item['startDateTime']);
            item.end = new Date(item['endDateTime']);
          });
          this.events = calendarItems;
        });
    } else {
      this.events = [];
    }
  }

  setDateTimeInterval(renderEvent: any) {
    if (this.startDate.getTime() !== renderEvent.period.start.getTime()) {
      this.startDate = renderEvent.period.start;
      this.endDate = renderEvent.period.end;
      this.updateCalendarItems();
    }
  }

  beforeMonthViewRender(renderEvent: CalendarMonthViewBeforeRenderEvent) {
    this.setDateTimeInterval(renderEvent);
  }

  beforeWeekViewRender(renderEvent: CalendarWeekViewBeforeRenderEvent) {
    this.setDateTimeInterval(renderEvent);
  }

  beforeDayViewRender(renderEvent: CalendarDayViewBeforeRenderEvent) {
    this.setDateTimeInterval(renderEvent);
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (date.getMonth() === this.viewDate.getMonth()) {
      if (
        (this.viewDate.getDay() === date.getDay() && this.isActiveDayOpen === true) ||
        events.length === 0
      ) {
        this.isActiveDayOpen = false;
      } else {
        this.isActiveDayOpen = true;
      }
      this.viewDate = date;
    }
  }

  closeOpenMonthViewDay() {
    this.isActiveDayOpen = false;
  }
}
