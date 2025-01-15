import { Component } from '@angular/core';
import { AuthService } from '../../service/auth.service'; // Make sure the path is correct
import { Router } from '@angular/router';
import { ReactiveFormsModule , FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-registration',
  imports: [ReactiveFormsModule, CommonModule, MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule ],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css',
})
export class RegistrationComponent {
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    // Create the form group
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      registrationDate: ['', Validators.required],
    });
  }

  // Registration form submission
  onRegister() {
    if (this.registerForm.invalid) {
      return; // Prevent submitting the form if invalid
    }

    // Get the form value
    const user = this.registerForm.value;

    // Call the auth service to register the user
    this.authService.register(user).subscribe(
      (response) => {
        this.router.navigate(['/login']); // Navigate to the login page on successful registration
      },
      (error) => {
        console.error(error); // Handle error in registration
      }
    );
  }
}
