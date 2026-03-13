import { Route } from '@angular/router';

import { authGuard } from './core/auth/auth.guard';
import { CallbackComponent } from './pages/callback/callback.component';
import { HomeComponent } from './pages/home/home.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';

export const appRoutes: Route[] = [
  { path: '', component: HomeComponent, canActivate: [authGuard] },
  { path: 'callback', component: CallbackComponent },
  { path: '**', component: NotFoundComponent },
];
