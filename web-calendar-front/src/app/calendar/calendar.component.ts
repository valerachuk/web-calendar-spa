import { Component, OnInit } from '@angular/core';
import { CalendarItemsService } from '../services/calendar-items.service';
import {
  CalendarDayViewBeforeRenderEvent,
  CalendarEvent,
  CalendarMonthViewBeforeRenderEvent,
  CalendarView,
  CalendarWeekViewBeforeRenderEvent
} from 'angular-calendar';
import {
  isSameDay,
  isSameMonth
} from 'date-fns';
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
  isEventModified = false;

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
    if (this.selectedCalendars.length > 0 || this.isEventModified) {
      var timeInterval = [this.startDate.toJSON(), this.endDate.toJSON()];
      this.calendarComponentService.getCalendarsItems(timeInterval, this.selectedCalendars)
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
    if (this.startDate.getTime() !== renderEvent.period.start.getTime() || this.isEventModified) {
      this.startDate = renderEvent.period.start;
      this.endDate = renderEvent.period.end;
      this.isEventModified = false;
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
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.isActiveDayOpen === true) ||
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
