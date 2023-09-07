import { IAirport } from "./iairport.interface";

export interface IAirportStatusResponse {
  statusCode: number;
  value: IAirport;
}
