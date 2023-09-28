import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { IFlight } from '../../../interfaces/iflight.interface';
import { FlightType } from '../../../types/flight.type';

@Component({
  selector: 'flight-list',
  templateUrl: './flight-list.component.html',
  styleUrls: ['./flight-list.component.scss']
})
export class FlightListComponent {

  @Input() flights?: Observable<IFlight[]>;
  @Input() hideFlightType?: boolean;
  @Input() title?: string;
  @Input() flightType?: FlightType;

  trackByFlightId(index: number, flight: IFlight) {
    return flight.flightId;
  }

  filter = (flight: IFlight) => flight.flightType === this.flightType
}
