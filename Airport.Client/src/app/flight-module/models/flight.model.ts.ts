import { IFlight } from "../../interfaces/iflight.interface";
import { FlightType } from "../../types/flight.type";

export class Flight implements IFlight {

  constructor(
    public flightId: string,
    public stationId: string | undefined,
    public flightType: FlightType,
    public color: string) { }
}
