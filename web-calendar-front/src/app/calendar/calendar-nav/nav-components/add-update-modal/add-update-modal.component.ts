import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';

@Component({
  selector: 'app-add-update-modal',
  templateUrl: './add-update-modal.component.html',
  styleUrls: ['./add-update-modal.component.css']
})
export class AddUpdateModalComponent implements OnInit {
  public form = new FormGroup({
    calendarName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    calendarDesc: new FormControl(null, Validators.maxLength(1000))
  });
  calendarName = this.form.get('calendarName');
  calendarDesc = this.form.get('calendarDesc');

  public errors = [];
  public addedNewCalendar = false;

  @Input() isCalendarOwner: boolean;
  @Input() calendar: Calendar;

  public addMode: boolean;

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    if(typeof this.calendar === 'undefined' || typeof this.isCalendarOwner === 'undefined') {
      this.addMode = true;
    }
    else {
      this.calendarName.setValue(this.calendar.name);
      this.calendarDesc.setValue(this.calendar.description);
      if(!this.isCalendarOwner)
        this.notOwner();
      this.addMode = false;
    }
  }

  notOwner() {
    this.errors.push("Not calendar owner");
    setTimeout(()=> this.activeModal.dismiss(), 1000);
  }

  addCalendar() {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errors = [];
    this.form.disable();
    var newCalendar: Calendar = {
      id: 0,
      name: this.form.value.calendarName,
      description: this.form.value.calendarDesc,
      userId: this.authService.userId
    }
    this.calendarService.addCalendar(newCalendar).subscribe(calendar => {
      this.addedNewCalendar = true;
      setTimeout(() => this.activeModal.close(calendar), 500);
    }, err => {
      if (err.status == 400)
        this.errors.push("Calendar not added");
    })
    .add(()=> {
      this.form.reset();
      this.form.enable();
    });
  }

  editCalendar() {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errors = [];
    this.form.disable();
    var newCalendar: Calendar = {
      id: this.calendar.id,
      name: this.calendarName.value,
      description: this.calendarDesc.value,
      userId: this.calendar.userId
    }
    this.calendarService.editCalendar(newCalendar).subscribe(calendar => {
      this.activeModal.close(calendar);
    }, err => {
      if(err.status === 403) {
        this.isCalendarOwner = false;
        this.notOwner();
      }
      else if(err.status === 404)
        this.errors.push("Calendar not found");
    })
    .add(()=> {
      setTimeout(() => this.activeModal.dismiss(), 500);
    });
  }
}
