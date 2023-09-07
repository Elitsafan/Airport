import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private dataSubject: BehaviorSubject<any>;
  private hubConnection: signalR.HubConnection | undefined;
  data$?: Observable<any>;

  constructor() {
    this.dataSubject = new BehaviorSubject<any>(null!);
    this.data$ = this.dataSubject.asObservable();
  }

  public startConnection = async () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.remoteUrl}${environment.airportHubEP}`)
      .build();
    await this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.addStationChangedListener(data => this.dataSubject.next(JSON.parse(data)))
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public addStationChangedListener(listener: (...args: any[]) => any) {
    if (!this.hubConnection)
      throw new Error("Connection didn't start yet")
    this.hubConnection?.on('StationChanged', listener);
  }
}
