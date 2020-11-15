import { Component, OnInit } from '@angular/core';
import { CalendarItemsService } from '../services/calendar-items.service';
import {
  CalendarDayViewBeforeRenderEvent,
  CalendarEvent,
  CalendarEventTimesChangedEvent,
  CalendarMonthViewBeforeRenderEvent,
  CalendarView,
  CalendarWeekViewBeforeRenderEvent
} from 'angular-calendar';
import * as moment from 'moment';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DeleteItemModalComponent } from './calendar-nav/nav-components/delete-item-modal/delete-item-modal.component';
import { ItemType } from '../enums/calendar-item-type.enum';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { EventFormComponent } from './calendar-nav/nav-components/event-form/event-form.component';


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
  faTrash = faTrash;
  faEdit = faEdit;

  viewDate: Date = new Date();
  events: CalendarEvent[] = [];

  startDate: Date;
  endDate: Date;

  selectedCalendars = [];

  constructor(
    private calendarComponentService: CalendarItemsService,
    private modalService: NgbModal) { }

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
            item.id = item['id'];
            item.title = item['name'];
            item.start = new Date(item['startDateTime']);
            item.end = new Date(item['endDateTime']);
            item.meta = item['metaType'] as ItemType;
            item.draggable = true;
            item.resizable = {
              beforeStart: item.meta == ItemType.Event || item.meta == ItemType.RepeatableEvent ? true : false,
              afterEnd: item.meta == ItemType.Event || item.meta == ItemType.RepeatableEvent ? true : false
            }
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

  openEventModal(event: CalendarEvent) {
    if (event.meta === ItemType.RepeatableEvent || event.meta as ItemType === ItemType.Event) {
      let modalRef = this.modalService.open(EventFormComponent, { centered: true })
      modalRef.componentInstance.getcalendarEvent(event.id);
      modalRef.result
        .then(_ => {
          this.updateCalendarItems();
          this.closeOpenMonthViewDay();
        }, () => { });
    }
  }

  openDeleteItemModal(calendarItem: CalendarEvent) {
    let modalRef = this.modalService.open(DeleteItemModalComponent, { centered: true, size: 'sm' });
    modalRef.componentInstance.item = calendarItem;
    modalRef.result.then(() => {
      this.updateCalendarItems();
      this.closeOpenMonthViewDay();
    });
  }

  eventTimesChanged({
    event,
    newStart,
    newEnd,
  }: CalendarEventTimesChangedEvent): void {
    if(+event.start === +newStart && +event.end === +newEnd)
      return;

    event.start = newStart;
    event.end = newEnd;

    this.calendarComponentService.updateCalendarItem(event).subscribe(_ => {
      this.updateCalendarItems();
      this.closeOpenMonthViewDay();
    }
    );
  }
}
