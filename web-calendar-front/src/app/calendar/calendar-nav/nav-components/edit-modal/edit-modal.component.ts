import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { CalendarService } from 'src/app/services/calendar.service';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.css']
})
export class EditModalComponent implements OnInit {
  @Input() isCalendarOwner: boolean;
  @Input() calendar: Calendar;

  public editForm = new FormGroup({
    calendarName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    calendarDesc: new FormControl(null, Validators.maxLength(1000))
  });
  calendarName = this.editForm.get('calendarName');
  calendarDesc = this.editForm.get('calendarDesc');
  public errors = [];

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService
  ) { }

  ngOnInit(): void {
    this.calendarName.setValue(this.calendar.name);
    this.calendarDesc.setValue(this.calendar.description);
    if(!this.isCalendarOwner)
      this.notOwner();
  }

  notOwner() {
    this.errors.push("Not calendar owner");
    setTimeout(()=> this.activeModal.dismiss(), 1000);
  }

  editCalendar() {
    if (!this.editForm.valid) {
      this.editForm.markAllAsTouched();
      return;
    }
    this.editForm.disable();
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
      else
        this.errors.push(err);
    }, () => {
      this.editForm.reset();
      this.editForm.enable();
    });
  }

}
