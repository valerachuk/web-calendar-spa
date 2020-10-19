import { Component } from '@angular/core';
import {
  CalendarEvent,
  CalendarView
} from 'angular-calendar';
import * as moment from 'moment';

@Component({
  selector: 'calendar-component',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent {
  Moment = moment.updateLocale('en', {
    week: {
    dow: 1,
    },
  });
  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;

  viewDate: Date = new Date();
  events: CalendarEvent[] = [

  ];

  setView(view: CalendarView) {
    this.view = view;
  }
}
