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
    title: 'Dashboard',
    route: '/admin/dashboard',
    icon: 'bi bi-house',
    roles: ['admin', 'employee']
  },
  {
    title: 'Incidents',
    route: '/admin/incident-list',
    icon: 'bi bi-building',
    roles: ['admin', 'employee']
  },
  
];

