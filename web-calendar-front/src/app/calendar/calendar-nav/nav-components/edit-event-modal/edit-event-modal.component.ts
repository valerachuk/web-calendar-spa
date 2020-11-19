import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarEvent } from 'angular-calendar';
import { ItemType } from 'src/app/enums/calendar-item-type.enum';

@Component({
  selector: 'app-edit-event-modal',
  templateUrl: './edit-event-modal.component.html',
  styleUrls: ['./edit-event-modal.component.css']
})
export class EditEventModalComponent implements OnInit {

  @Input() item: CalendarEvent;
  isEventRepeatable = false;
  repeatableEvent = ItemType.RepeatableEvent;

  constructor(
    public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

  changeCheck() {
    this.isEventRepeatable = !this.isEventRepeatable;
  }

  editCalendarEvent() {
   this.activeModal.close(this.isEventRepeatable);
  }
}
