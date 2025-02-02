import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CinemaSelectorComponent } from './cinema-selector.component';

describe('CinemaSelectorComponent', () => {
  let component: CinemaSelectorComponent;
  let fixture: ComponentFixture<CinemaSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CinemaSelectorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CinemaSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
