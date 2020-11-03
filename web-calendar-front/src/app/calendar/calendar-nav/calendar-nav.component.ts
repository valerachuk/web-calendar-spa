import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarService } from 'src/app/services/calendar.service';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { EventFormComponent } from './nav-components/event-form/event-form.component';
import { AddModalComponent } from './nav-components/add-modal/add-modal.component';
import { DeleteModalComponent } from './nav-components/delete-modal/delete-modal.component';
import { CalendarComponent } from '../calendar.component';
import { faEdit, faPlus, faSearch, faTrash } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})

export class CalendarNavComponent implements OnInit {
  public calendars: Calendar[];
  public userName = this.authService.firstName;
  selectedItems = [];
  
  faPlus = faPlus;
  faTrash = faTrash;
  faPencil = faEdit;
  faSearch = faSearch;
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
        this.calendars = [...this.calendars, closeData];
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
      this.calendars = [...this.calendars];
      index = this.selectedItems.findIndex(calendar => calendar.id === closeData);
      this.selectedItems.splice(index, 1);
      this.selectedItems = [...this.selectedItems];
    }, () => { });
  }

  calendarIsChecked(calendar: number) {
    return this.selectedItems.includes(calendar);
  }

  setSelectedCalendars(calendar: number) {
    if (this.selectedItems.includes(calendar)) {
      this.selectedItems = this.selectedItems.filter(x => x !== calendar);
    } else {
      this.selectedItems = [...this.selectedItems, calendar];
    }
    this.updateCalendarItems();
  }

  updateCalendarItems() {
    this.calendarService.getCalendarsItems(this.selectedItems)
      .subscribe(calendarItems => {
        var calendarMatrix = {} as CalendarComponent;
        calendarMatrix.events = calendarItems;
      });
  }
}
