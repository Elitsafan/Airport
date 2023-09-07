import { IFlight } from "../../interfaces/iflight.interface";

export class Departure implements IFlight {

  readonly flightType = 'Departure';
  readonly color: string = 'bg-danger';

  constructor(
    public flightId: string,
    public stationId: number | undefined) { }
}
