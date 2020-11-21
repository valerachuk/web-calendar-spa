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

  uploadFile(file: File): Observable<number> {
    let formData = new FormData();
    formData.append("file", file, file.name);
    return this.httpClient.post<number>(this.apiUrl, formData);
  }

  getEventFile(eventId: number): Observable<HttpResponse<Blob>> {
    return this.httpClient.get(`${this.apiUrl}/${eventId}`, {responseType: 'blob', observe: 'response'});
  }
}
