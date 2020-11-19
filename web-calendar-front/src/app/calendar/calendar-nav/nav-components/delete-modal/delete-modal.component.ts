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
  @Input() isCalendarOwner: boolean;
  @Input() calendarId: number;

  constructor(
    public activeModal: NgbActiveModal,
    private calendarService: CalendarService
  ) { }

  ngOnInit(): void {
  }

  deleteCalendar() {
    this.calendarService.delete(this.calendarId).subscribe(id => {
      this.activeModal.close(id);
    }, err => {
      if(err.status === 403) {
        this.isCalendarOwner = false;
      }
    }).add(()=> {
      setTimeout(()=> this.activeModal.dismiss(), 2000); 
    });
  }
}
