import { IFlight } from "../../interfaces/iflight.interface";

export class Landing implements IFlight {

  readonly flightType = 'Landing';
  readonly color: string = 'bg-custom-yellow';

  constructor(
    public flightId: string,
    public stationId: number | undefined) { }
}
