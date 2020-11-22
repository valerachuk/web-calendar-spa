import { TestBed } from '@angular/core/testing';

import { FileAttachService } from './file-attach.service';

describe('FileAttachService', () => {
  let service: FileAttachService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FileAttachService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
