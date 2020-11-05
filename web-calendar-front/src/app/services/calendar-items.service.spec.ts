import { TestBed } from '@angular/core/testing';

import { CalendarItemsService } from './calendar-items.service';

describe('CalendarItemsService', () => {
  let service: CalendarItemsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CalendarItemsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
