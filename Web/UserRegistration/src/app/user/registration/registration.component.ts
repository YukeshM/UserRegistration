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
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-registration',
  providers: [AuthService],
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    FormsModule,
    RouterModule,
    MatIconModule,
    MatSnackBarModule,
  ],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css',
})
export class RegistrationComponent {
  registerForm: FormGroup;
  hidePassword = true;
  selectedFileName: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }

    this.registerForm = this.fb.group({
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
        ],
      ],
      lastName: [
        '',
        [
          Validators.required,
          Validators.minLength(1),
          Validators.maxLength(50),
        ],
      ],
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[a-zA-Z\d\W_]{6,}$/
          ),
        ],
      ],
      registrationDate: ['', Validators.required],
      document: [null, Validators.required],
    });
  }

  get form() {
    return this.registerForm.controls;
  }

  onRegister() {
    if (this.registerForm.invalid) {
      this.snackBar.open('Please fill in all fields correctly.', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar'],
        verticalPosition: 'top',
      });
      return;
    }

    const formData = new FormData();
    formData.append('Username', this.registerForm.get('username')?.value);
    formData.append('LastName', this.registerForm.get('lastName')?.value);
    formData.append('Email', this.registerForm.get('email')?.value);
    formData.append('Password', this.registerForm.get('password')?.value);

    const registrationDate = this.registerForm.get('registrationDate')?.value;
    const formattedDate = new Date(registrationDate).toISOString();
    formData.append('RegistrationDate', formattedDate);

    formData.append('Document', this.registerForm.get('document')?.value);

    // Call the auth service to register the user
    this.authService.register(formData).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.snackBar.open(
            'Registration successful! Redirecting...',
            'Close',
            {
              duration: 3000,
              panelClass: ['success-snackbar'],
              verticalPosition: 'top',
            }
          );
          this.router.navigate(['/login']);
        }
      },
      error: (error) => {
        console.error('Registration error:', error);
        this.snackBar.open(
          'Registration failed. Please contact administrator.',
          'Close',
          {
            duration: 3000,
            panelClass: ['error-snackbar'],
            verticalPosition: 'top',
          }
        );
      },
    });
  }

  // File upload handler
  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      if (file.size > 10 * 1024 * 1024) {
        this.snackBar.open('File size should not exceed 10MB.', 'Close', {
          duration: 3000,
          panelClass: ['error-snackbar'],
          verticalPosition: 'top',
        });
        return;
      }

      this.registerForm.patchValue({ document: file });
      this.selectedFileName = file.name; // Add this line
      console.log('File selected:', file.name);
    }
  }

  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
}
