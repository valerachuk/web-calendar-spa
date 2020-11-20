import { HttpBackend, HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FileAttachService {
  private apiUrl = environment.apiUrl + "/file";
  private httpClient: HttpClient;
  constructor(handler: HttpBackend) {
    this.httpClient = new HttpClient(handler);
  }

  uploadFile(file: File, eventId: number): Observable<FormData> {
    let formData = new FormData();
    formData.append("file", file, file.name);
    formData.append("eventId", eventId.toString());
    return this.httpClient.post<FormData>(this.apiUrl, formData);
  }

  getEventFile(eventId: number): Observable<HttpResponse<Blob>> {
    return this.httpClient.get(`${this.apiUrl}/${eventId}`, {responseType: 'blob', observe: 'response'});
  }
}
