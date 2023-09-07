import { Component, OnInit } from '@angular/core';
import { map, Observable } from 'rxjs';
import { StationService } from '../../../services/station.service';
import { Station } from '../../models/station.model';

@Component({
  selector: 'app-station-index',
  templateUrl: './station-index.component.html',
  styleUrls: ['./station-index.component.scss']
})

export class StationIndexComponent implements OnInit {

  middleStations$?: Observable<Station[]>;
  commonStations$?: Observable<Station[]>;
  endStationsDeparture$?: Observable<Station[]>;
  endStationsLanding$?: Observable<Station[]>;
  stations$?: Observable<Station[]>;

  constructor(private stationSvc: StationService) {
    this.stations$ = stationSvc.stations$;
  }

  async ngOnInit(): Promise<void> {
    await this.stationSvc.startService();
    await this.stationSvc.loadData();
    // Maps stations
    this.commonStations$ = this.stationSvc.stations$
      ?.pipe(
        map(stations => stations?.filter(
          e => e.stationId === 4 || e.stationId === 6 || e.stationId === 7)));
    this.endStationsLanding$ = this.stationSvc.stations$
      ?.pipe(
        map(stations => stations?.filter(
          e => e.stationId < 4)));
    this.endStationsDeparture$ = this.stationSvc.stations$
      ?.pipe(
        map(stations => stations?.filter(
          e => e.stationId === 9)));
    this.middleStations$ = this.stationSvc.stations$
      ?.pipe(
        map(stations => stations?.filter(
          e => e.stationId === 5 || e.stationId === 8)));
  }
}
