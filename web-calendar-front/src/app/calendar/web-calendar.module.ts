import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import * as moment from 'moment';
import { adapterFactory } from 'angular-calendar/date-adapters/moment';

import { WebCalendarRoutingModule } from './web-calendar-routing.module';
import { EventFormComponent } from './calendar-nav/nav-components/event-form/event-form.component';
import { AddUpdateModalComponent } from './calendar-nav/nav-components/add-update-modal/add-update-modal.component';
import { DeleteModalComponent } from './calendar-nav/nav-components/delete-modal/delete-modal.component';
import { CalendarComponent } from '../calendar/calendar.component';
import { CalendarNavComponent } from './calendar-nav/calendar-nav.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgSelectModule } from '@ng-select/ng-select';
import { DeleteItemModalComponent } from './calendar-nav/nav-components/delete-item-modal/delete-item-modal/delete-item-modal.component';

export function momentAdapterFactory() {
  return adapterFactory(moment);
};

@NgModule({
  declarations: [
    EventFormComponent,
    AddUpdateModalComponent,
    DeleteModalComponent,
    CalendarComponent,
    CalendarNavComponent,
    DeleteItemModalComponent
  ],
  exports: [EventFormComponent],
  entryComponents: [EventFormComponent],
  imports: [
    CommonModule,
    WebCalendarRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgSelectModule,
    FontAwesomeModule,
    CalendarModule.forRoot({ provide: DateAdapter, useFactory: momentAdapterFactory })
  ]
})
export class WebCalendarModule { }
