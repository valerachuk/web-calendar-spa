import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CalendarService } from '../services/calendar.service';
import { Calendar } from '../interfaces/calendar.interface';

@Component({
  selector: 'app-calendar-nav',
  templateUrl: './calendar-nav.component.html',
  styleUrls: ['./calendar-nav.component.css']
})
export class CalendarNavComponent implements OnInit {
  public userId: number;
  public calendars: Calendar[];

  public addCalendarForm = new FormGroup({
    newCalendarName: new FormControl(null, Validators.required),
    newCalendarDesc: new FormControl(null)
  });
  public addedNewCalendar = false;

  constructor(
    private modalService: NgbModal,
    private calendarService: CalendarService
  ) { }

  ngOnInit(): void {
    this.calendarService.get().subscribe(data => {
      this.calendars = data;
    });
  }

  openModal(content) {
   this.modalService.open(content, { centered: true });
  }

  addCalendar(){
    if (!this.addCalendarForm.valid) {
      this.addCalendarForm.markAllAsTouched();
      return;
    }
    this.addCalendarForm.disable();
    var newCalendar: Calendar = {
      id: 0,
      name: this.addCalendarForm.value.newCalendarName,
      description: this.addCalendarForm.value.newCalendarDesc,
      userId: this.calendarService.userId
    }
    this.calendarService.addCalendar(newCalendar).subscribe(calendar => {
      this.calendars.push(calendar);
      this.addedNewCalendar = true;
      this.addCalendarForm.reset();
      this.addCalendarForm.enable();
    });
  }
}
