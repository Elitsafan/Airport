import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { Flight } from '../flight-module/models/flight.model.ts';
import { IFlight } from '../interfaces/iflight.interface';
import { Station } from '../station-module/models/station.model';
import { AirportService } from './airport.service';
import { ColorService } from './color.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class FlightService implements OnDestroy {

  flights$?: Observable<Flight[]>;
  private flights: Flight[];
  private startSubscription?: Subscription;
  private statusSubscription?: Subscription;
  private flightsSubscription?: Subscription;
  private dataChangedSubscription?: Subscription;
  private flightsSubject = new BehaviorSubject<Flight[]>([]);

  constructor(
    private airportSvc: AirportService,
    private signalRSvc: SignalrService,
    private colorSvc: ColorService
  ) {
    this.flights = [];
    this.flights$ = this.flightsSubject.asObservable();
    this.startSubscription = this.airportSvc.start().subscribe({
      next: () => {
        this.statusSubscription = this.fetch()
      }, error: error => {
        console.error(error);
      }
    });
    this.dataChangedSubscription = this.signalRSvc.stationChangedData$
      ?.subscribe((data: Station[]) => {
        data?.forEach(s => {
          const flight = this.flightResolver(s.flight);
          if (flight)
            this.handleFlight(flight);
        })
      })
  }

  private handleFlight(flight: IFlight) {
    this.upsertFlight(flight, this.flights);
    this.flightsSubject.next(this.flights);
  }

  ngOnDestroy(): void {
    this.startSubscription?.unsubscribe();
    this.statusSubscription?.unsubscribe();
    this.dataChangedSubscription?.unsubscribe();
    this.flightsSubscription?.unsubscribe();
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
        // populates departures
        this.flights = airport.departures.map(flight => new Flight(
          flight.flightId!,
          flight.stationId,
          flight.flightType,
          this.colorSvc.getColor(
            flight.flightId,
            flight.flightType)));
        const landings = airport.landings.map(flight => new Flight(
          flight.flightId!,
          flight.stationId,
          flight.flightType,
          this.colorSvc.getColor(
            flight.flightId,
            flight.flightType)));
        // adds landings
        this.flights.push(...landings);
        // Triggers initial flights
        this.flightsSubject.next(this.flights);
      },
      error: (error: HttpErrorResponse) => {
        console.log(error)
      }
    });
  }

  private flightResolver(flight?: IFlight) {
    return flight
      ? new Flight(
        flight.flightId,
        flight.stationId,
        flight.flightType,
        this.colorSvc.getColor(flight.flightId, flight.flightType))
      : undefined;
  }
}
