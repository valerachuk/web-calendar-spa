import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUpdateModalComponent } from './add-update-modal.component';

describe('AddUpdateModalComponent', () => {
  let component: AddUpdateModalComponent;
  let fixture: ComponentFixture<AddUpdateModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddUpdateModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddUpdateModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
