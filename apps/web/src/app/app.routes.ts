import { Route } from '@angular/router';

import { NotFoundComponent } from './pages/not-found.component';

export const appRoutes: Route[] = [
  { path: '**', component: NotFoundComponent },
];
