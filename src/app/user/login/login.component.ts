import { Component } from '@angular/core';
import { AuthService } from '../../service/auth.service'; // Make sure the path is correct
import { Router } from '@angular/router';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  FormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule, MatSnackBarModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]], // Add email validator
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(/(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]/), // At least one uppercase, one lowercase, and one number
        ],
      ],
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
      });
      return;
    }

    const credentials = this.loginForm.value;

    this.authService.login(credentials).subscribe({
      next: (response: any) => {

        console.log(response);

        if(response.success && response.data != "" && response.data) {
          this.authService.storeToken(response.data);

        // Show success message using Snackbar
        this.snackBar.open('Login successful! Redirecting...', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar'],
        });

        // Navigate to home page after successful login
        this.router.navigate(['/home']);
        }else{
          this.snackBar.open(
            'Login failed. Please check your credentials and try again.',
            'Close',
            {
              duration: 3000,
              panelClass: ['error-snackbar'],
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
          }
        );
      },
    });
  }
}
