import { TestBed } from '@angular/core/testing';

import { ToastGlobalService } from './toast-global.service';

describe('ToastGlobalService', () => {
  let service: ToastGlobalService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ToastGlobalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
