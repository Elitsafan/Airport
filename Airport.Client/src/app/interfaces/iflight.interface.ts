import { FlightType } from "../types/flight.type";

export interface IFlight {
  flightId: string;
  stationId: number | undefined;
  flightType: FlightType;
  color: string; 
}
