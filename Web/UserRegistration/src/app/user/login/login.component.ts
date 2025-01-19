import { Component } from '@angular/core';
import { AuthService } from '../../service/auth.service';
import { Router, RouterModule } from '@angular/router';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  FormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    FormsModule,
    MatSnackBarModule,
    RouterModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
    }

    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  get form() {
    return this.loginForm.controls;
  }

  onLogin() {
    if (this.loginForm.invalid) {
      this.snackBar.open('Please fill in all fields correctly.', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar'],
        verticalPosition: 'top',
      });
      return;
    }

    const credentials = this.loginForm.value;

    this.authService.login(credentials).subscribe({
      next: (response: any) => {
        console.log(response);

        if (
          response.success &&
          response.data != '' &&
          response.data &&
          response.data.token != ''
        ) {
          this.authService.storeToken(response.data.token);

          // Show success message using Snackbar
          this.snackBar.open('Login successful! Redirecting...', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar'],
            verticalPosition: 'top', // Display snackbar at the top
          });

          // Navigate to home page after successful login
          this.router.navigate(['/home']);
        } else {
          this.snackBar.open(
            'Login failed. Please check your credentials and try again.',
            'Close',
            {
              duration: 3000,
              panelClass: ['error-snackbar'],
              verticalPosition: 'top', // Display snackbar at the top
            }
          );
        }
      },
      error: (error) => {
        // Handle error and show error message
        this.snackBar.open(
          'Login failed. Please check your credentials and try again.',
          'Close',
          {
            duration: 3000,
            panelClass: ['error-snackbar'],
            verticalPosition: 'top', // Display snackbar at the top
          }
        );
      },
    });
  }

  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
}
