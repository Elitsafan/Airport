import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { map } from 'rxjs';
import { IAirportStatusResponse } from '../interfaces/iairport-status-response.interface';
import { IAirportSummaryResponse } from '../interfaces/iairport-summary-response.interface';

@Injectable({
  providedIn: 'root'
})
export class AirportService {

  constructor(private http: HttpClient) { }

  // Gets the current status of airport
  getStatus() {
    return this.http.get<IAirportStatusResponse>(
      `${environment.remoteUrl}${environment.statusEP}`,
      { observe: 'body' })
      .pipe(map(data => data.value));
  }

  // Gets information of all the flights
  getSummary() {
    return this.http.get<IAirportSummaryResponse>(
      `${environment.remoteUrl}${environment.summaryEP}`,
      { observe: 'body' })
      .pipe(map(data => data.value));
  }

  // Starts the airport
  start() {
    return this.http.get<HttpResponse<any>>(
      `${environment.remoteUrl}${environment.startEP}`,
      { observe: 'body' });
  }
}
