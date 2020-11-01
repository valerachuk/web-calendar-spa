import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { EventFormComponent } from './nav-components/event-form/event-form.component';
import { AddModalComponent } from './nav-components/add-modal/add-modal.component';
import { DeleteModalComponent } from './nav-components/delete-modal/delete-modal.component';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})

export class CalendarNavComponent implements OnInit {
  public addModal = AddModalComponent;
  public deleteModal = DeleteModalComponent;

  public calendars: Calendar[];
  public userName = this.authService.firstName;

  constructor(
    private modalService: NgbModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }
 
  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendarService.calendars = data;
      this.calendars = this.calendarService.calendars;
    });
  }

  openEventModal() {
    this.modalService.open(EventFormComponent, { centered: true });
  }
  
  openModal(content, mdSize) {
    this.modalService.open(content, { centered: true, size: mdSize});
  }

  openDeleteModal(calendarId) {
    let modalRef = this.modalService.open(this.deleteModal, { centered: true, size: 'sm'});
    modalRef.componentInstance.calendarId = calendarId;
  }
}
