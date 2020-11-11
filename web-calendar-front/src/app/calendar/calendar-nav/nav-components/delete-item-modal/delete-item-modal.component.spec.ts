import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteItemModalComponent } from './delete-item-modal.component';

describe('DeleteItemModalComponent', () => {
  let component: DeleteItemModalComponent;
  let fixture: ComponentFixture<DeleteItemModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeleteItemModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteItemModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
