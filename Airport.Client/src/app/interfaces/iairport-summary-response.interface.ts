import { FlightSummary } from "../flight-summary-module/models/flight-summary.model";

export interface IAirportSummaryResponse {
  statusCode: number;
  value: FlightSummary[];
}
