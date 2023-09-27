import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { IFlight } from '../interfaces/iflight.interface';
import { FlightType } from '../types/flight.type';

@Injectable({
  providedIn: 'root'
})
export class ColorService {

  private landingColorIndex: number;
  private departureColorIndex: number;
  private landingColors: string[];
  private departureColors: string[];
  private dictionary: Map<string, string>;

  constructor() {
    this.dictionary = new Map();
    this.landingColorIndex = 0;
    this.departureColorIndex = 0;
    this.landingColors = environment.landingColors;
    this.departureColors = environment.departureColors;
  }

  getColor(flightId: string, flightType: FlightType) {
    const value = this.dictionary.get(flightId);
    if (value)
      return value;
    const color = flightType === "Departure"
      ? this.getNextDepartureColor()
      : this.getNextLandingColor();
    console.log(color);
    this.dictionary.set(flightId, color);
    return color;
  }

  private getNextLandingColor() {
    return this.landingColors[this.landingColorIndex++ % this.landingColors.length];
  }

  private getNextDepartureColor() {
    return this.departureColors[this.departureColorIndex++ % this.departureColors.length]
  }
}
