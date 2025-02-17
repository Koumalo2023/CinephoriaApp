export interface NavigationItem {
  title: string;
  route?: string;
  icon: string;
  children?: NavigationItem[];
  isOpen?: boolean;
  roles: string[];
}

export const NAVIGATION_ITEMS: NavigationItem[] = [
  
  {
    title: 'Incidents',
    route: '/admin/incident-list',
    icon: 'bi bi-building',
    roles: ['admin', 'employee']
  },
  {
    title: 'Déconnexion',
    icon: 'bi bi-box-arrow-right', 
    roles: ['admin', 'employee'],
    route: '/logout'
  }
  
];

