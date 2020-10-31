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

  addEvent(calendarEvent: CalendarEvent): Observable<string> {
     return this.httpClient.post<string>(this.apiUrl, calendarEvent);
  }
}
