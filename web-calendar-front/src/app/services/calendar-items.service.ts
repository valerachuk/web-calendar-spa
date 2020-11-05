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

  getCalendarsItems(timeInterval: string[], selectedCalendars: number[]): Observable<CalendarEvent[]> {
    const paramObject = {
      timeInterval: timeInterval.map(interval => interval),
      id: selectedCalendars.map(calendatId => calendatId)
    };

    return this.httpClient.get<CalendarEvent[]>(this.apiUrl + "?", { params: <any>paramObject });
  }
}
