import { Station } from "../station-module/models/station.model";
import { IFlight } from "./iflight.interface";

export interface IAirport {
  landings: IFlight[];
  departures: IFlight[];
  stations: Station[]
}
