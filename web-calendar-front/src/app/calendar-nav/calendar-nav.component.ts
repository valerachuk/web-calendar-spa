import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CalendarService } from '../services/calendar.service';
import { Calendar } from '../interfaces/calendar.interface';
import { AuthService } from '../services/auth.service';
import { EventFormComponent } from '../event-form/event-form.component';
import { CalendarComponent } from '../calendar/calendar.component';


@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})

export class CalendarNavComponent implements OnInit {
  @ViewChild('delCalendar') deleteModal: any;
  isCalendarOwner: boolean;
  deleteCalendarId: number;


  public calendars = [];
  public userName = this.authService.firstName;

  public addCalendarForm = new FormGroup({
    newCalendarName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    newCalendarDesc: new FormControl(null, Validators.maxLength(1000))
  });


  public addedNewCalendar = false;
  public errors = [];
  calendarName = this.addCalendarForm.get('newCalendarName');
  calendarDesc = this.addCalendarForm.get('newCalendarDesc');

  selectedItems = [];
  dropdownSettings = {};

  constructor(
    private modalService: NgbModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.calendarService.get(this.authService.userId).subscribe(data => {
      this.calendars = data;
    });

    this.selectedItems = [];
    this.dropdownSettings = {
      singleSelection: false,
      text: "",
      selectAllText: 'Select all',
      unSelectAllText: 'UnSelect all',
      enableSearchFilter: true,
      showCheckbox: true,
      noDataLabel: "Cannot find calendar",
      maxHeight: 150,
      searchPlaceholderText: "Calendar name"
    };
  }

  calendarIsChecked(calendar: Calendar) {
    return this.selectedItems.includes(calendar);
  }

  setSelectedCalendars(calendar: Calendar) {
    if (this.selectedItems.includes(calendar)) {
      this.selectedItems = this.selectedItems.filter(x => x.id !== calendar.id);
    } else {
      this.selectedItems.push(calendar);
    }
    this.UpdateCalendarItems();
  }

  UpdateCalendarItems() {
    console.log(this.selectedItems);
    this.calendarService.getCalendarsItems(this.selectedItems)
      .subscribe(calendarItems => {
        var calendarMatrix = {} as CalendarComponent;
        calendarMatrix.events = calendarItems;
      },
        err => {
          if (err.status == 400)
            this.errors.push("Error code 400, calendars items was not found");
        });
  }

  openEventModal() {
    this.modalService.open(EventFormComponent, { centered: true });
  }

  openModal(content, mdSize) {
    this.modalService.open(content, { centered: true, size: mdSize });
  }

  confirmDeleteCalendar(calendarId: number) {
    if (this.calendars.find(c => c.id === calendarId).userId !== this.authService.userId) {
      this.isCalendarOwner = false;
      this.openModal(this.deleteModal, 'sm');
      return;
    }
    this.isCalendarOwner = true;
    this.deleteCalendarId = calendarId;
    this.openModal(this.deleteModal, 'sm');
  }

  deleteCalendar(id: number) {
    this.calendarService.delete(id).subscribe(id => {
      let index = this.calendars.findIndex(calendar => calendar.id === id);
      this.calendars.splice(index, 1);
    }, err => {
      if (err.status === 403) {
        this.isCalendarOwner = false;
        this.openModal(this.deleteModal, 'sm');
      }
    });
  }

  addCalendar() {
    if (!this.addCalendarForm.valid) {
      this.addCalendarForm.markAllAsTouched();
      return;
    }
    this.addCalendarForm.disable();
    var newCalendar: Calendar = {
      id: 0,
      name: this.addCalendarForm.value.newCalendarName,
      description: this.addCalendarForm.value.newCalendarDesc,
      userId: this.authService.userId
    }
    this.calendarService.addCalendar(newCalendar).subscribe(calendar => {
      this.calendars.push(calendar);
      this.addedNewCalendar = true;
      setTimeout(() => this.addedNewCalendar = false, 2500);
    }, err => {
      if (err.status == 400)
        this.errors.push("Error code 400, calendar not added");
    }, () => {
      this.addCalendarForm.reset();
      this.addCalendarForm.enable();
    });
  }
}
