import { Component } from '@angular/core';
import { AuthService } from '../../service/auth.service'; // Make sure the path is correct
import { Router } from '@angular/router';
import { ReactiveFormsModule ,FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true, 
  imports: [ReactiveFormsModule, CommonModule ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onLogin() {
    if (this.loginForm.invalid) {
      this.errorMessage = 'Please fill out all required fields';
      return;
    }

    const credentials = this.loginForm.value;
    this.authService.login(credentials).subscribe(
      (response: any) => {
        this.authService.storeToken(response.token);
        this.router.navigate(['/home']);
      },
      (error) => {
        this.errorMessage = 'Invalid credentials';
      }
    );
  }
}