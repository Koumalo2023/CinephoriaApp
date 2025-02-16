// src/app/shared/pipes/date-format.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'dateFormat',
  standalone: true,
})
export class DateFormatPipe implements PipeTransform {
  // Utilisez DatePipe d'Angular pour formater la date
  private datePipe: DatePipe = new DatePipe('en-US');

  transform(value: string | Date, format: string = 'dd/MM/yyyy HH:mm'): string | null {
    if (!value) {
      return null;
    }

    // Si la valeur est une chaîne, la convertir en objet Date
    if (typeof value === 'string') {
      value = new Date(value);
    }

    // Formater la date
    return this.datePipe.transform(value, format);
  }
}