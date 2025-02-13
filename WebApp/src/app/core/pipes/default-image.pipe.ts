import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  standalone: true,
  name: 'defaultImage'
})
export class DefaultImagePipe implements PipeTransform {
  transform(imageUrl: string | null | undefined, defaultImageUrl: string): string {
    if (!imageUrl || imageUrl.trim() === '') {
      return defaultImageUrl;
    }
    try {
      new URL(imageUrl); 
      return imageUrl;
    } catch (error) {
      return defaultImageUrl;
    }
  }
}