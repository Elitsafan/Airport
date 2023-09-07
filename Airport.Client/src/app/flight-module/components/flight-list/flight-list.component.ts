import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { IFlight } from '../../../interfaces/iflight.interface';

@Component({
  selector: 'flight-list',
  templateUrl: './flight-list.component.html',
  styleUrls: ['./flight-list.component.scss']
})
export class FlightListComponent {

  @Input() flights?: Observable<IFlight[]>;
  @Input() hideFlightType?: boolean;
  @Input() title?: string;

  trackByFlightId(index: number, flight: IFlight) {
    return flight.flightId;
  }
}
