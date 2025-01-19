import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../service/auth.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-home',
  imports: [MatIconModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  currentYear: number = new Date().getFullYear();

  constructor(private router: Router, private authService: AuthService) {}

  logout() {
    // Use AuthService to log out the user
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
