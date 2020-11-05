import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { EventFormComponent } from './nav-components/event-form/event-form.component';
import { AddUpdateModalComponent } from './nav-components/add-update-modal/add-update-modal.component';
import { DeleteModalComponent } from './nav-components/delete-modal/delete-modal.component';
import { faEdit, faPlus, faSearch, faTrash } from '@fortawesome/free-solid-svg-icons';
import { CalendarComponent } from '../calendar.component';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})

export class CalendarNavComponent implements OnInit {
  public calendars: Calendar[];
  public userName = this.authService.firstName;
  selectedCalendars = [];

  faPlus = faPlus;
  faTrash = faTrash;
  faPencil = faEdit;
  faSearch = faSearch;

  constructor(
    private modalService: NgbModal,
    private calendarService: CalendarService,
    private authService: AuthService,
    public calendarComponent: CalendarComponent
  ) { }

  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });
  }

  openEventModal() {
    this.modalService.open(EventFormComponent, { centered: true }).result
      .then(closeData => {
        this.updateCalendarItems();
      }, () => { });;
  }

  openAddModal() {
    this.modalService.open(AddUpdateModalComponent, { centered: true, size: 'md'}).result
      .then(closeData => {
        this.calendars = [...this.calendars, closeData];
      }, () => { });
  }

  openDeleteModal(calendarId) {
    let modalRef = this.modalService.open(DeleteModalComponent, { centered: true, size: 'sm' });
    if (this.calendars.find(c => c.id === calendarId).userId !== this.authService.userId) {
      modalRef.componentInstance.isCalendarOwner = false;
      return;
    }
    modalRef.componentInstance.isCalendarOwner = true;
    modalRef.componentInstance.calendarId = calendarId;
    modalRef.result.then(closeData => {
      let index = this.calendars.findIndex(calendar => calendar.id === closeData);
      this.calendars.splice(index, 1);
      this.calendars = [...this.calendars];
      index = this.selectedCalendars.findIndex(calendar => calendar.id === closeData);
      this.selectedCalendars.splice(index, 1);
      this.selectedCalendars = [...this.selectedCalendars];
      this.updateCalendarItems();
    }, () => { });
  }

  openEditModal(calendar) {
    let modalRef = this.modalService.open(AddUpdateModalComponent, { centered: true, size: 'md'});
    if(calendar.userId !== this.authService.userId) {
      modalRef.componentInstance.isCalendarOwner = false;
      return;
    }
    modalRef.componentInstance.isCalendarOwner = true;
    modalRef.componentInstance.calendar = calendar;
    modalRef.result.then(closeData => {
      this.calendars.splice(this.calendars.findIndex(c => c.id === calendar.id), 1, closeData);
    }, () => { });
  }
  
  calendarIsChecked(calendarId: number) {
    return this.selectedCalendars.includes(calendarId);
  }

  setSelectedCalendars(calendarId: number) {
    if (this.selectedCalendars.includes(calendarId)) {
      this.selectedCalendars = this.selectedCalendars.filter(x => x !== calendarId);
    } else {
      this.selectedCalendars = [...this.selectedCalendars, calendarId];
    }
    this.updateCalendarItems();
  }

  updateCalendarItems() {
    this.calendarComponent.selectedCalendars = this.selectedCalendars;
    this.calendarComponent.updateCalendarItems();
  }
}
