import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Calendar } from '../interfaces/calendar.interface';
import { HttpClient } from '@angular/common/http';
import { MyIdComponent } from '../my-id/my-id.component';
import { Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private apiUrl = environment.apiUrl + "/calendar/";
  public userId: number;
  constructor(private httpClient: HttpClient) {
    this.userId = new MyIdComponent(httpClient).id;
  }

  get(): Observable<Calendar[]> {
    return this.httpClient.get(`${environment.apiUrl}/auth/id`).pipe(
      map(id => {
        this.userId = +id;
        return id;
      }), 
      mergeMap(id => this.httpClient.get<Calendar[]>(this.apiUrl + id))
    );
  }

  addCalendar(calendar: Calendar): Observable<Calendar> {
    return this.httpClient.post<Calendar>(this.apiUrl, calendar);
  }
}
