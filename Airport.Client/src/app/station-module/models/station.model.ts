import { IFlight } from "../../interfaces/iflight.interface";

export class Station {
  constructor(
    public stationId: number,
    public flight?: IFlight) { }
}
