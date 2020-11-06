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

  getCalendarsItems(startDate: string, endDate: string,  selectedCalendars: number[]): Observable<CalendarEvent[]> {
    const paramObject = {
      start: startDate,
      end: endDate,
      id: selectedCalendars.map(calendatId => calendatId)
    };
    return this.httpClient.get<CalendarEvent[]>(this.apiUrl + "?", { params: <any>paramObject });
  }
}
