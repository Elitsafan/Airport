import { FlightType } from "../types/flight.type";

export interface IFlight {
  flightId: string;
  stationId: string | undefined;
  flightType: FlightType;
  color: string; 
}
