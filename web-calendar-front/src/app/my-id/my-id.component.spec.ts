import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyIdComponent } from './my-id.component';

describe('MyIdComponent', () => {
  let component: MyIdComponent;
  let fixture: ComponentFixture<MyIdComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyIdComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MyIdComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
