import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { FlightService } from '../../../services/flight.service';
import { Departure } from '../../models/departure.model.ts';
import { Landing } from '../../models/landing.model.ts';

@Component({
  selector: 'flights-index',
  templateUrl: './flights-index.component.html',
  styleUrls: ['./flights-index.component.scss']
})
export class FlightsIndexComponent {

  landings$?: Observable<Landing[]>;
  departures$?: Observable<Departure[]>;

  constructor(private flightSvc: FlightService) {
    this.landings$ = this.flightSvc.landings$;
    this.departures$ = this.flightSvc.departures$;
  }
}
