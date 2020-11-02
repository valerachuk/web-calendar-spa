import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Calendar } from '../interfaces/calendar.interface';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CalendarEvent } from 'angular-calendar';

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

  getCalendarsItems(selectedCalendars: Calendar[]): Observable<CalendarEvent[]>{
    var params = "Items?";
    for(let calendar of selectedCalendars){
      params += "id=" + calendar.id+"&"; 
    }
    return this.httpClient.get<CalendarEvent[]>(this.apiUrl+params.slice(0, -1));
  }

  addCalendar(calendar: Calendar): Observable<Calendar> {
    return this.httpClient.post<Calendar>(this.apiUrl, calendar);
  }

  delete(id: number): Observable<number> {
    return this.httpClient.delete<number>(this.apiUrl + id);
  }
}
