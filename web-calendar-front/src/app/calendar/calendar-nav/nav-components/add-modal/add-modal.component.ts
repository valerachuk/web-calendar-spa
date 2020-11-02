import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';

@Component({
  selector: 'app-add-modal',
  templateUrl: './add-modal.component.html',
  styleUrls: ['./add-modal.component.css']
})
export class AddModalComponent implements OnInit {

  public addCalendarForm = new FormGroup({
    newCalendarName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    newCalendarDesc: new FormControl(null, Validators.maxLength(1000))
  });
  public addedNewCalendar = false;
  public errors = [];
  calendarName = this.addCalendarForm.get('newCalendarName');
  calendarDesc = this.addCalendarForm.get('newCalendarDesc');

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
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
      this.addedNewCalendar = true;
      setTimeout(() => this.activeModal.close(calendar), 500);
    }, err => {
      if (err.status == 400)
        this.errors.push("Error code 400, calendar not added");
    }, () => {
      this.addCalendarForm.reset();
      this.addCalendarForm.enable();
    });
  }
}
