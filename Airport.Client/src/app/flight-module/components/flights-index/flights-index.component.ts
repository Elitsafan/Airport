import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { FlightService } from '../../../services/flight.service';
import { FlightType } from '../../../types/flight.type';
import { Flight } from '../../models/flight.model.ts';

@Component({
  selector: 'flights-index',
  templateUrl: './flights-index.component.html',
  styleUrls: ['./flights-index.component.scss']
})
export class FlightsIndexComponent {

  flights$?: Observable<Flight[]>;
  departure: FlightType;
  landing: FlightType;

  constructor(private flightSvc: FlightService) {
    this.flights$ = this.flightSvc.flights$;
    this.departure = "Departure";
    this.landing = "Landing";
  }
}
