import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FlightsIndexComponent } from './flights-index.component';

describe('FlightsIndexComponent', () => {
  let component: FlightsIndexComponent;
  let fixture: ComponentFixture<FlightsIndexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FlightsIndexComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FlightsIndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
