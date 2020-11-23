import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Calendar } from '../interfaces/calendar.interface';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private apiUrl = environment.apiUrl + "/calendar/";

  constructor(private httpClient: HttpClient) {
  }

  get(id: number): Observable<Calendar[]> {
    return this.httpClient.get<Calendar[]>(this.apiUrl + id);
  }

  addCalendar(calendar: Calendar): Observable<Calendar> {
    return this.httpClient.post<Calendar>(this.apiUrl, calendar);
  }

  delete(id: number): Observable<number> {
    return this.httpClient.delete<number>(this.apiUrl + id);
  }

  editCalendar(calendar: Calendar): Observable<Calendar> {
    return this.httpClient.put<Calendar>(this.apiUrl, calendar);
  }

  downloadCalendarIcs(calendarId: number): Observable<HttpResponse<Blob>> {
    return this.httpClient.get(`${this.apiUrl}calendar-ics/${calendarId}`, {
      responseType: 'blob',
      observe: 'response'
    });
  }
}
