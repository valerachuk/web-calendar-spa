import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-my-id',
  templateUrl: './my-id.component.html',
  styleUrls: ['./my-id.component.css']
})
export class MyIdComponent implements OnInit {

  public id: number;
  private apiUrl = environment.apiUrl;

  constructor(
    private httpClient: HttpClient
  ) { }

  ngOnInit(): void {
    this.httpClient.get(`${this.apiUrl}/auth/id`)
      .subscribe(id => {
        this.id = +id;
      })
  }

}
