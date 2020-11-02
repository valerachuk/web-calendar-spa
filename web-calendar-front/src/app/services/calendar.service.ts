import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Calendar } from '../interfaces/calendar.interface';
import { HttpClient } from '@angular/common/http';
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
}
