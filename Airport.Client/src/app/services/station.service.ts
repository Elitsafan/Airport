import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, lastValueFrom, Observable, Subscription } from 'rxjs';
import { Departure } from '../flight-module/models/departure.model.ts';
import { Landing } from '../flight-module/models/landing.model.ts';
import { IFlight } from '../interfaces/iflight.interface';
import { Station } from '../station-module/models/station.model';
import { AirportService } from './airport.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})

export class StationService implements OnDestroy {
  stations$?: Observable<Station[]>;
  private stations: Station[];
  private stationsSubscription?: Subscription;
  private stationsSubject = new BehaviorSubject<Station[]>([]);

  constructor(private airportSvc: AirportService, private signalRSvc: SignalrService) {
    this.stations = [];
    this.stations$ = this.stationsSubject.asObservable();
  }

  public loadData(): Promise<void> {
    return this.fetch()
  }

  public startService(): Promise<void> {
    return lastValueFrom(this.airportSvc.start())
      .then(() => this.handleStationsSubscription())
      .catch((error) => console.error(error))
  }

  ngOnDestroy(): void {
    this.stationsSubscription?.unsubscribe();
  }

  private fetch() {
    return lastValueFrom(this.airportSvc.getStatus())
      .then((airport) => {
        this.stations = airport!.stations.map(
          station => new Station(
            station.stationId,
            this.flightResolver(station.flight)));
        this.stationsSubject.next(this.stations);
      })
      .catch((error: HttpErrorResponse) => console.error(error));
  }

  private flightResolver(flight?: IFlight) {
    return flight
      ? flight.flightType === 'Departure'
        ? new Departure(flight.flightId, flight.stationId)
        : new Landing(flight.flightId, flight.stationId)
      : undefined;
  }

  private handleStationsSubscription() {
    this.stationsSubscription = this.signalRSvc.data$
      ?.subscribe((stations: Station[]) => {
        this.stations?.forEach((station, i) => station.flight = this.flightResolver(stations[i].flight))
        this.stationsSubject.next(this.stations);
      })
  }
}
