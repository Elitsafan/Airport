import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StationIndexComponent } from './station-index.component';

describe('StationIndexComponent', () => {
  let component: StationIndexComponent;
  let fixture: ComponentFixture<StationIndexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StationIndexComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StationIndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
