import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { FlightSummary } from '../flight-summary-module/models/flight-summary.model';
import { StationSummary } from '../flight-summary-module/models/station-summary.model';
import { AirportService } from './airport.service';

@Injectable({
  providedIn: 'root'
})
export class FlightSummaryService implements OnDestroy {
  summaries$: Observable<FlightSummary[]>;
  private flightsSubject = new BehaviorSubject<FlightSummary[]>([]);
  private startSubscription?: Subscription;
  private summarySubscription?: Subscription;

  constructor(private airportSvc: AirportService) {
    this.summaries$ = this.flightsSubject.asObservable();
    this.startSubscription = this.airportSvc.start()
      .subscribe({
        next: () => {
          this.summarySubscription = this.fetch();
        },
        error: error => {
          console.error(error);
        }
      })
  }

  ngOnDestroy(): void {
    this.startSubscription?.unsubscribe();
    this.summarySubscription?.unsubscribe();
  }

  // Gets flights summeries
  private fetch() {
    return this.airportSvc.getSummary()
      .subscribe({
        next: (summaries) => {
          const flightsSummary = summaries.map(record => new FlightSummary(
            record.flightId,
            record.flightType,
            record.stations.map(station => new StationSummary(
              station.stationId,
              station.entrance,
              station.exit))))
          this.flightsSubject.next(flightsSummary);
        },
        error: (error: HttpErrorResponse) => {
          console.log(error)
        }
      });
  }
}
