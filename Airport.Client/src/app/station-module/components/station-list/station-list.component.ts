import { Component, Input } from '@angular/core';
import { Station } from '../../models/station.model';

@Component({
  selector: 'station-list',
  templateUrl: './station-list.component.html',
  styleUrls: ['./station-list.component.scss']
})
export class StationListComponent {
  @Input() stations?: Station[];

  trackByStationId(index: number, station: Station): string {
    return station.stationId;
  }
}
