import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarEvent } from 'angular-calendar';
import { ItemType } from 'src/app/enums/calendar-item-type.enum';
import { CalendarEventService } from 'src/app/services/calendar-event.service';

@Component({
  selector: 'app-delete-item-modal',
  templateUrl: './delete-item-modal.component.html',
  styleUrls: ['./delete-item-modal.component.css']
})
export class DeleteItemModalComponent implements OnInit {

  @Input() item: CalendarEvent;
  isEventRepeatable = true;
  repeatableEvent = ItemType.RepeatableEvent;
  error = "";
  public btnDisabled = false;

  constructor(
    public activeModal: NgbActiveModal,
    private eventService: CalendarEventService) { }

  ngOnInit(): void {
  }

  changeCheck() {
    this.isEventRepeatable = !this.isEventRepeatable;
  }

  deleteSingleEvent() {
    this.eventService.deleteEvent(+this.item.id).subscribe(id => {
      this.activeModal.close(id)},
      err => {
        this.error = err;
      }
    );
  }

  deleteEventSeries() {
    this.eventService.deleteEventSeries(+this.item.id).subscribe(id => {
      this.activeModal.close(id)
    },
      err => {
        this.error = err;
      }
    );
  }

  deleteCalendarItem() {
    this.btnDisabled = true;
    switch (this.item.meta) {
      case ItemType.Event:
        this.deleteSingleEvent();
        break;
      case ItemType.RepeatableEvent:
        if (this.isEventRepeatable) {
          this.deleteEventSeries();
        } else {
          this.deleteSingleEvent();
        }
        break;
      case ItemType.Task:
        // delete task
        break;
      case ItemType.Reminder:
        //delete reminder
        break;
    }
  }
}
