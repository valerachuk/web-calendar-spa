import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-not-found-page',
  templateUrl: './not-found-page.component.html',
  styleUrls: ['./not-found-page.component.css']
})
export class NotFoundPageComponent implements OnInit {
  public href: string;
  constructor(private router: Router) {
    this.router.events.subscribe((event: NavigationEnd) => {
      if(event.url === "/404")
        this.href = '';
      else 
        this.href = event.url;
    });
   }

  ngOnInit(): void {
  }

}
