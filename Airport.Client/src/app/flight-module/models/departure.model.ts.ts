import { IFlight } from "../../interfaces/iflight.interface";

export class Departure implements IFlight {

  readonly flightType = 'Departure';

  constructor(
    public flightId: string,
    public stationId: string | undefined,
    public color: string) { }
}
