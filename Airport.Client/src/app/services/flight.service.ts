import { Time } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Departure } from '../flight-module/models/departure.model.ts';
import { Landing } from '../flight-module/models/landing.model.ts';
import { IFlight } from '../interfaces/iflight.interface';
import { Station } from '../station-module/models/station.model';
import { AirportService } from './airport.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class FlightService implements OnDestroy {

  landings$?: Observable<Landing[]>;
  departures$?: Observable<Departure[]>;
  private departures: Departure[];
  private landings: Landing[];
  private startSubscription?: Subscription;
  private statusSubscription?: Subscription;
  private departuresSubscription?: Subscription;
  private landingsSubscription?: Subscription;
  private dataChangedSubscription?: Subscription;
  private landingsSubject = new BehaviorSubject<Landing[]>([]);
  private departuresSubject = new BehaviorSubject<Departure[]>([]);

  constructor(private airportSvc: AirportService, private signalRSvc: SignalrService) {
    this.landings = [];
    this.departures = [];
    this.landings$ = this.landingsSubject.asObservable();
    this.departures$ = this.departuresSubject.asObservable();
    this.startSubscription = this.airportSvc.start().subscribe({
      next: () => {
        this.statusSubscription = this.fetch()
      }, error: error => {
        console.error(error);
      }
    });
    this.dataChangedSubscription = this.signalRSvc.data$
      ?.subscribe((data: Station[]) => {
        data.forEach(s => {
          const flight = this.flightResolver(s.flight);
          // Departure
          if (flight?.flightType === 'Departure') {
            this.handleFlight(this.departuresSubject, flight, this.departures);
            // Landing
          } else if (flight?.flightType === 'Landing') {
            this.handleFlight(this.landingsSubject, flight, this.landings);
          }
        })
      })
  }

  private handleFlight(subject: BehaviorSubject<any>, flight: IFlight, flights: IFlight[]) {
    this.upsertFlight(flight, flights);
    subject.next(flights);
  }

  ngOnDestroy(): void {
    this.startSubscription?.unsubscribe();
    this.statusSubscription?.unsubscribe();
    this.dataChangedSubscription?.unsubscribe();
    this.departuresSubscription?.unsubscribe();
    this.landingsSubscription?.unsubscribe();
  }

  private upsertFlight(flight: IFlight, flights: IFlight[]) {
    const index = flights.findIndex(d => d.flightId === flight.flightId);
    // If flight exists
    if (index < 0)
      flights.push(flight);
    // New Flight
    else
      flights[index] = flight;
  }

  private fetch() {
    return this.airportSvc.getStatus().subscribe({
      next: (airport) => {
        this.landings = airport.landings.map(flight => new Landing(flight.flightId!, flight.stationId));
        this.departures = airport.departures.map(flight => new Departure(flight.flightId!, flight.stationId));
        // Triggers initial flights
        this.departuresSubject.next(this.departures);
        this.landingsSubject.next(this.landings);
      },
      error: (error: HttpErrorResponse) => {
        console.log(error)
      }
    });
  }

  private flightResolver(flight?: IFlight) {
    return flight
      ? flight.flightType === 'Departure'
        ? new Departure(flight.flightId, flight.stationId)
        : new Landing(flight.flightId, flight.stationId)
      : undefined;
  }
}
