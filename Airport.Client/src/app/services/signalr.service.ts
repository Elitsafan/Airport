import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private stationChangedSubject: BehaviorSubject<any>;
  private flightRunDoneSubject: BehaviorSubject<any>;
  private hubConnection: signalR.HubConnection | undefined;
  stationChangedData$?: Observable<any>;
  flightRunDoneData$: Observable<any>;

  constructor() {
    this.stationChangedSubject = new BehaviorSubject<any>(null!);
    this.flightRunDoneSubject = new BehaviorSubject<any>(null!);
    this.stationChangedData$ = this.stationChangedSubject.asObservable();
    this.flightRunDoneData$ = this.flightRunDoneSubject.asObservable();
  }

  public startConnection = async () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.remoteUrl}${environment.airportHubEP}`)
      .build();
    await this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.addStationChangedListener(data => this.stationChangedSubject.next(JSON.parse(data)))
        this.addFlightRunDoneListener(data => this.flightRunDoneSubject.next(JSON.parse(data)))
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  // Adds a listener to station changed event
  public addStationChangedListener(listener: (...args: any[]) => any) {
    if (!this.hubConnection)
      throw new Error("Connection didn't start yet")
    this.hubConnection?.on('StationChanged', listener);
  }

  // Adds a listener to flight run done event
  public addFlightRunDoneListener(listener: (...args: any[]) => any) {
    if (!this.hubConnection)
      throw new Error("Connection didn't start yet")
    this.hubConnection?.on('FlightRunDone', listener);
  }
}
