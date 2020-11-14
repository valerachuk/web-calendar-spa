import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Calendar } from 'src/app/interfaces/calendar.interface';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';
import { ToastGlobalService } from 'src/app/services/toast-global.service';

@Component({
  selector: 'app-add-update-modal',
  templateUrl: './add-update-modal.component.html',
  styleUrls: ['./add-update-modal.component.css']
})
export class AddUpdateModalComponent implements OnInit {
  public form = new FormGroup({
    calendarName: new FormControl(null, [Validators.required, Validators.maxLength(100), this.noWhitespaceValidator]),
    calendarDesc: new FormControl(null, Validators.maxLength(1000))
  });
  calendarName = this.form.get('calendarName');
  calendarDesc = this.form.get('calendarDesc');

  public errors = [];
  public addedNewCalendar = false;

  @Input() isCalendarOwner: boolean;
  @Input() calendar: Calendar;

  public addMode: boolean;
  public modalTitle: string;

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private authService: AuthService,
    private toastService: ToastGlobalService
  ) { }

  ngOnInit(): void {
    if (typeof this.calendar === 'undefined' || typeof this.isCalendarOwner === 'undefined') {
      this.addMode = true;
      this.modalTitle = "Add new calendar";
    }
    else {
      this.calendarName.setValue(this.calendar.name);
      this.calendarDesc.setValue(this.calendar.description);
      if (!this.isCalendarOwner)
        this.notOwner();
      this.addMode = false;
      this.modalTitle = "Edit calendar";
    }
  }

  public noWhitespaceValidator(control: FormControl) {
    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace;
    return isValid ? null : { 'whitespace': true };
}

  notOwner() {
    this.errors.push("Not calendar owner");
    setTimeout(() => this.activeModal.dismiss(), 1000);
  }

  submit() {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errors = [];
    this.form.disable();
    let newCalendar: Calendar = {
      id: this.addMode ? 0 : this.calendar.id,
      name: this.form.value.calendarName,
      description: this.form.value.calendarDesc,
      userId: this.authService.userId
    }
    if (this.addMode)
      this.addCalendar(newCalendar);
    else
      this.editCalendar(newCalendar);
  }

  addCalendar(newCalendar) {
    this.calendarService.addCalendar(newCalendar).subscribe(calendar => {
      this.addedNewCalendar = true;
      this.toastService.add({
        delay: 5000,
        title: 'Success!',
        content: 'Calendar was added successfully',
        className: "bg-success text-light"
      });
      setTimeout(() => this.activeModal.close(calendar), 500);
    }, err => {
      if (err.status == 400)
        this.errors.push("Calendar was not added");
    })
      .add(() => {
        this.form.reset();
        this.form.enable();
      });
  }

  editCalendar(newCalendar) {
    this.calendarService.editCalendar(newCalendar).subscribe(calendar => {
      this.activeModal.close(calendar);
      this.toastService.add({
        delay: 5000,
        title: 'Success!',
        content: 'Calendar was edited successfully',
        className: "bg-success text-light"
      });
    }, err => {
      if (err.status === 403) {
        this.isCalendarOwner = false;
        this.notOwner();
      }
      else if (err.status === 404)
        this.errors.push("Calendar was not found");
    })
      .add(() => {
        setTimeout(() => this.activeModal.dismiss(), 500);
      });
  }
}
