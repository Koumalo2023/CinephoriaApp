import { CommonModule } from '@angular/common';
import { Component, ContentChild, ElementRef, Input, TemplateRef } from '@angular/core';

@Component({
  selector: 'app-container',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './container.component.html',
  styleUrl: './container.component.scss'
})
export class ContainerComponent {
  
  @Input() cardTitle!: string; 

@Input() cardClass!: string;

@Input() showHeader = true;

@Input() showContent = true;

@Input() blockClass!: string;

@Input() headerClass!: string;

@Input() footerClass!: string;

@Input() padding!: number; // set default to 24 px

@ContentChild('headerOptionsTemplate') headerOptionsTemplate!: TemplateRef<ElementRef>;

@ContentChild('headerTitleTemplate') headerTitleTemplate!: TemplateRef<ElementRef>;

@ContentChild('footerTemplate') footerTemplate!: TemplateRef<ElementRef>;

}
