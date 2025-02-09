import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageSeatsComponent } from './manage-seats.component';

describe('ManageSeatsComponent', () => {
  let component: ManageSeatsComponent;
  let fixture: ComponentFixture<ManageSeatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageSeatsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageSeatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
