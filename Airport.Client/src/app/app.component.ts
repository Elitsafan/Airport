import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AirportService } from './services/airport.service';
import { SignalrService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private startSubscription?: Subscription;

  constructor(private airportSvc: AirportService, private signalRSvc: SignalrService) { }

  ngOnDestroy(): void {
    this.startSubscription?.unsubscribe();
  }

  ngOnInit(): void {
    this.startSubscription = this.airportSvc.start()
      .subscribe({
        next: async (res) => {
          await this.signalRSvc.startConnection();
          //console.log(res);
        },
        error: (error) => {
          console.log(error)
        }
      });
  }
  title = 'Airport Client';
}
