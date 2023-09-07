import { TestBed } from '@angular/core/testing';

import { FlightSummaryService } from './flight-summary.service';

describe('FlightSummaryService', () => {
  let service: FlightSummaryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FlightSummaryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
