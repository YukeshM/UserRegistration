import { Routes } from '@angular/router';
import { LoginComponent } from './user/login/login.component';
import { RegistrationComponent } from './user/registration/registration.component';

export const routes: Routes = [ 
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegistrationComponent },
    { path: '', redirectTo: '/login', pathMatch: 'full' }
];
