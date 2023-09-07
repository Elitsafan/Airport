import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StationComponent } from './components/station/station.component';
import { StationListComponent } from './components/station-list/station-list.component';
import { StationIndexComponent } from './components/station-index/station-index.component';
import { FlightModule } from '../flight-module/flight.module';

@NgModule({
  declarations: [
    StationComponent,
    StationListComponent,
    StationIndexComponent],
  imports: [
    CommonModule,
    FlightModule
  ],
  exports: [StationIndexComponent]
})
export class StationModule { }
