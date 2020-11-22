import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileAttachComponent } from './file-attach.component';

describe('FileAttachComponent', () => {
  let component: FileAttachComponent;
  let fixture: ComponentFixture<FileAttachComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FileAttachComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FileAttachComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
