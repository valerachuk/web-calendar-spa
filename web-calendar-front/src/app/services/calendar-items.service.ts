import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CalendarEvent } from 'angular-calendar';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})

export class CalendarItemsService {
  private apiUrl = environment.apiUrl + "/calendaritem";

  constructor(private httpClient: HttpClient) {
  }

  getCalendarsItems(startDate: string, endDate: string, selectedCalendars: number[]): Observable<CalendarEvent[]> {
    const paramObject = {
      start: startDate,
      end: endDate,
      id: selectedCalendars.map(calendatId => calendatId)
    };
    return this.httpClient.get<CalendarEvent[]>(this.apiUrl + "?", { params: <any>paramObject });
  }

  updateCalendarItem(event: CalendarEvent): Observable<string> {
    var timeOffsetInMS = new Date().getTimezoneOffset() * 60000;
    const sendEvent = {
      id: event.id,
      name: event.title,
      startDateTime: new Date(event.start.setTime(event.start.getTime() - timeOffsetInMS)).toJSON(),
      endDateTime: new Date(event.end.setTime(event.end.getTime() - timeOffsetInMS)).toJSON(),
      metaType: event.meta
    }
    return this.httpClient.put<string>(this.apiUrl, sendEvent);
  }
}
