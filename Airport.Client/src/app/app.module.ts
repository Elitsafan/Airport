import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { FlightModule } from './flight-module/flight.module';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { StationModule } from './station-module/station.module';
import { FlightSummaryModule } from './flight-summary-module/flight-summary.module';

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FlightModule,
    StationModule,
    FlightSummaryModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
