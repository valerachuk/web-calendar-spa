import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CalendarEvent } from '../interfaces/event.interface';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CalendarEventService {

  private apiUrl = environment.apiUrl + "/event";

  constructor(private httpClient: HttpClient) { }

  getEvent(calendarEventId: number): Observable<CalendarEvent> {
    return this.httpClient.get<CalendarEvent>(this.apiUrl + `?id=${calendarEventId}`);;
  }

  addEvent(calendarEvent: CalendarEvent): Observable<string> {
    return this.httpClient.post<string>(this.apiUrl, calendarEvent);
  }

  updateEvent(calendarEvent: CalendarEvent): Observable<string> {
    return this.httpClient.put<string>(this.apiUrl, calendarEvent);
  }

  updateEventSeries(calendarEvent: CalendarEvent): Observable<string> {
    return this.httpClient.put<string>(this.apiUrl + '/EditEventSeries', calendarEvent);
  }

  deleteEvent(id: number): Observable<string> {
    return this.httpClient.delete<string>(this.apiUrl + `?id=${id}`);
  }

  deleteEventSeries(id: number): Observable<string> {
    return this.httpClient.delete<string>(this.apiUrl + `/DeleteSeries?id=${id}`);
  }

  unsubscribeEvent(id: number): Observable<string> {
    return this.httpClient.delete<string>(this.apiUrl + `/Unsubscribe?id=${id}`);
  }

  unsubscribeEventSeries(id: number): Observable<string> {
    return this.httpClient.delete<string>(this.apiUrl + `/UnsubscribeSeries?id=${id}`);
  }
}
