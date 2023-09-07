import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { FlightSummary } from '../../models/flight-summary.model';
import { FlightSummaryService } from '../../../services/flight-summary.service';
import { FlightType } from '../../../types/flight.type';

@Component({
  selector: 'flight-summary-list',
  templateUrl: './flight-summary-list.component.html',
  styleUrls: ['./flight-summary-list.component.scss']
})

export class FlightSummaryListComponent {

  flights$: Observable<FlightSummary[]>;
  constructor(private flightSummarySvc: FlightSummaryService) {
    this.flights$ = flightSummarySvc.summaries$;
  }

  filterFlightType = (value: FlightType) => {
    return (flight: FlightSummary) => flight.flightType === value
  }

  trackByFlightId(index: number, flight: FlightSummary) {
    return flight.flightId;
  }
}
