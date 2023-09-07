import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { FlightsIndexComponent } from './flight-module/components/flights-index/flights-index.component';
import { FlightSummaryListComponent } from './flight-summary-module/components/flight-summary-list/flight-summary-list.component';
import { StationIndexComponent } from './station-module/components/station-index/station-index.component';

const routes: Routes = [
  { path: 'flights', component: FlightsIndexComponent },
  { path: 'stations', component: StationIndexComponent },
  { path: 'summary', component: FlightSummaryListComponent },
  { path: '', redirectTo: '/flights', pathMatch: 'full' }, 
  { path: '**', component: PageNotFoundComponent },  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
