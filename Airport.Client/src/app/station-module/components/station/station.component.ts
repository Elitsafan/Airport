import { Component, Input } from '@angular/core';
import { Station } from '../../models/station.model';

@Component({
  selector: 'station',
  templateUrl: './station.component.html',
  styleUrls: ['./station.component.scss']
})
export class StationComponent {
  @Input() station?: Station;
}
