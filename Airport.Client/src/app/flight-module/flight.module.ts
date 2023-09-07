import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlightsIndexComponent } from './components/flights-index/flights-index.component';
import { FlightComponent } from './components/flight/flight.component';
import { FlightListComponent } from './components/flight-list/flight-list.component';

@NgModule({
  declarations: [
    FlightComponent,
    FlightListComponent,
    FlightsIndexComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    FlightComponent,
    FlightsIndexComponent
  ]
})
export class FlightModule { }
