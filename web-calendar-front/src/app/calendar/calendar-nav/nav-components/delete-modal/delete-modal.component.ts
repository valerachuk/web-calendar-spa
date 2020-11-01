import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'src/app/services/auth.service';
import { CalendarService } from 'src/app/services/calendar.service';

@Component({
  selector: 'app-delete-modal',
  templateUrl: './delete-modal.component.html',
  styleUrls: ['./delete-modal.component.css']
})
export class DeleteModalComponent implements OnInit {
  deleteCalendarId: number;
  isCalendarOwner: boolean;
  @Input() calendarId: number;

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    if(this.calendarService.calendars.find(c => c.id === this.calendarId).userId !== this.authService.userId) {
      this.isCalendarOwner = false;
      return;
    }
    this.isCalendarOwner = true;
    this.deleteCalendarId = this.calendarId;
  }

  deleteCalendar(id: number) {
    this.calendarService.delete(id).subscribe(id => {
      let index = this.calendarService.calendars.findIndex(calendar => calendar.id === id);
      this.calendarService.calendars.splice(index, 1);
      this.activeModal.close();
    }, err => {
      if(err.status === 403) {
        this.isCalendarOwner = false;
      }
    });
  }
}
