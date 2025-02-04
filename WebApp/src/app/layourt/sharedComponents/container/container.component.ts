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
// Décorateur @Input pour permettre la liaison de données depuis un composant parent. 
// 'cardTitle' est une propriété d'entrée qui attend une chaîne de caractères pour le titre du card.

@Input() cardClass!: string;
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'cardClass' est une propriété d'entrée qui attend une chaîne de caractères pour définir une classe CSS personnalisée pour la carte elle-même.

@Input() showHeader = true;
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'showHeader' est une propriété d'entrée de type booléen qui détermine si l'en-tête de la carte doit être affiché. Par défaut, il est vrai (true).

@Input() showContent = true;
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'showContent' est une propriété d'entrée de type booléen qui détermine si le contenu de la carte doit être affiché. Par défaut, il est vrai (true).

@Input() blockClass!: string;
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'blockClass' est une propriété d'entrée qui attend une chaîne de caractères pour définir une classe CSS personnalisée pour un bloc spécifique à l'intérieur de la carte.

@Input() headerClass!: string;
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'headerClass' est une propriété d'entrée qui attend une chaîne de caractères pour définir une classe CSS personnalisée pour l'en-tête de la carte.

@Input() footerClass!: string;
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'footerClass' est une propriété d'entrée qui attend une chaîne de caractères pour définir une classe CSS personnalisée pour le pied de page du card.

@Input() padding!: number; // set default to 24 px
// Décorateur @Input pour permettre la liaison de données depuis un composant parent.
// 'padding' est une propriété d'entrée qui attend un nombre pour définir l'espacement intérieur (padding) du card. Un commentaire suggère de définir la valeur par défaut à 24 pixels.

@ContentChild('headerOptionsTemplate') headerOptionsTemplate!: TemplateRef<ElementRef>;
// Décorateur @ContentChild pour accéder au contenu projeté (ng-content) dans le composant.
// 'headerOptionsTemplate' est une référence au template Angular (TemplateRef) qui contient les options personnalisées pour l'en-tête du card.

@ContentChild('headerTitleTemplate') headerTitleTemplate!: TemplateRef<ElementRef>;
// Décorateur @ContentChild pour accéder au contenu projeté (ng-content) dans le composant.
// 'headerTitleTemplate' est une référence au template Angular (TemplateRef) qui contient le titre personnalisé pour l'en-tête du card.

@ContentChild('footerTemplate') footerTemplate!: TemplateRef<ElementRef>;
// Décorateur @ContentChild pour accéder au contenu projeté (ng-content) dans le composant.
// 'footerTemplate' est une référence au template Angular (TemplateRef) qui contient le contenu personnalisé pour le pied de page du card.

}
