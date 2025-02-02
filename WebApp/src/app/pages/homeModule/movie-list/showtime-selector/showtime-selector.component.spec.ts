import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowtimeSelectorComponent } from './showtime-selector.component';

describe('ShowtimeSelectorComponent', () => {
  let component: ShowtimeSelectorComponent;
  let fixture: ComponentFixture<ShowtimeSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShowtimeSelectorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShowtimeSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
