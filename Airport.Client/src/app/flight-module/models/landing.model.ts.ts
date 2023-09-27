import { IFlight } from "../../interfaces/iflight.interface";

export class Landing implements IFlight {

  readonly flightType = 'Landing';

  constructor(
    public flightId: string,
    public stationId: string | undefined,
    public color: string) { }
}
