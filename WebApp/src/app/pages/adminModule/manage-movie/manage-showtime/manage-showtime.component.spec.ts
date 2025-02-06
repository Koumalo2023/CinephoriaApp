import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageShowtimeComponent } from './manage-showtime.component';

describe('ManageShowtimeComponent', () => {
  let component: ManageShowtimeComponent;
  let fixture: ComponentFixture<ManageShowtimeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageShowtimeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageShowtimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
