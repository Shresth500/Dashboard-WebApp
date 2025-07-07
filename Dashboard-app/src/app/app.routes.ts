import { Routes } from '@angular/router';
import { AuthenticationComponent } from './Authentication/authentication/authentication.component';
import { LoginComponent } from './Authentication/login/login.component';
import { RegisterComponent } from './Authentication/register/register.component';
import { DashboardComponent } from './Dashboard/dashboard/dashboard.component';
import { AuthGuard } from './common/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthenticationComponent,
    children: [
      {
        path: 'login',
        component: LoginComponent,
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full',
      },
      {
        path: 'register',
        component: RegisterComponent,
      },
    ],
  },
  {
    path: '',
    component: DashboardComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'home',
    redirectTo: '',
    pathMatch: 'full',
  },
];
