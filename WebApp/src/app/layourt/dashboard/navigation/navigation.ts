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
    title: 'Films',
    route: '/admin/manage-movie',
    icon: 'bi bi-film',
    roles: ['admin', 'employee']
  },
  {
    title: 'Cinéma',
    route: '/admin/manage-cinema',
    icon: 'bi bi-building',
    roles: ['admin', 'employee']
  },
  {
    title: 'Réservations',
    route: '/admin/manage-reservation',
    icon: 'bi bi-ticket',
    roles: ['admin', 'employee']
  },
  {
    title: 'Employés',
    route: '/admin/manage-employee',
    icon: 'bi bi-people',
    roles: ['admin','employee']
  }
];

