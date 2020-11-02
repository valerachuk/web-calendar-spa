import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { EventFormComponent } from './nav-components/event-form/event-form.component';
import { AddModalComponent } from './nav-components/add-modal/add-modal.component';
import { DeleteModalComponent } from './nav-components/delete-modal/delete-modal.component';
import { EditModalComponent } from './nav-components/edit-modal/edit-modal.component';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})

export class CalendarNavComponent implements OnInit {
  public calendars: Calendar[];
  public userName = this.authService.firstName;

  constructor(
    private modalService: NgbModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }
 
  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
  }

  openEventModal() {
    this.modalService.open(EventFormComponent, { centered: true });
  }

  openAddModal() {
    this.modalService.open(AddModalComponent, { centered: true, size: 'md'}).result
      .then(closeData => {
        this.calendars.push(closeData);
      }, () => { });
  }

  openDeleteModal(calendarId) {
    let modalRef = this.modalService.open(DeleteModalComponent, { centered: true, size: 'sm'});
    if(this.calendars.find(c => c.id === calendarId).userId !== this.authService.userId) {
      modalRef.componentInstance.isCalendarOwner = false;
      return;
    }
    modalRef.componentInstance.isCalendarOwner = true;
    modalRef.componentInstance.calendarId = calendarId;
    modalRef.result.then(closeData => {
      let index = this.calendars.findIndex(calendar => calendar.id === closeData);
      this.calendars.splice(index, 1);
    }, () => { });
  }

  openEditModal(calendarId) {
    let modalRef = this.modalService.open(EditModalComponent, { centered: true, size: 'md'});
    let calendarIndex = this.calendars.findIndex(calendar => calendar.id === calendarId);
    if(this.calendars[calendarIndex].userId !== this.authService.userId) {
      modalRef.componentInstance.isCalendarOwner = false;
      return;
    }
    modalRef.componentInstance.isCalendarOwner = true;
    modalRef.componentInstance.calendar = this.calendars[calendarIndex];
    modalRef.result.then(closeData => {
      this.calendars.splice(calendarIndex, 1, closeData);
    }, () => { });
  }
}
