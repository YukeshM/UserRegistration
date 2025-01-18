import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { ReactiveFormsModule, FormsModule, FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../service/auth.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    const snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        FormsModule,
        MatIconModule,
        MatInputModule,
        MatButtonModule,
        RouterTestingModule,
        LoginComponent, // <-- Import the standalone LoginComponent directly here
      ],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: MatSnackBar, useValue: snackBarSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    snackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    fixture.detectChanges();
  });

  it('should create the login component', () => {
    expect(component).toBeTruthy();
  });

  it('should call AuthService.login with correct credentials on form submit', () => {
    // Arrange: Fill out the form with valid data
    component.loginForm.controls['email'].setValue('test@example.com');
    component.loginForm.controls['password'].setValue('Test1234!');
    fixture.detectChanges();

    // Mock the AuthService response
    authService.login.and.returnValue(
      of({ success: true, data: 'dummy-token' })
    );

    // Act: Call the onLogin method
    component.onLogin();

    // Assert: Check if login was called with correct credentials
    expect(authService.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'Test1234!',
    });

    // Assert: Check if the success snackbar is shown
    expect(snackBar.open).toHaveBeenCalledWith(
      'Login successful! Redirecting...',
      'Close',
      { duration: 3000, panelClass: ['success-snackbar'] }
    );

    // Assert: Check if navigation to home occurs
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('should show an error message when login fails', () => {
    // Arrange: Fill out the form with valid data
    component.loginForm.controls['email'].setValue('test@example.com');
    component.loginForm.controls['password'].setValue('Test1234!');
    fixture.detectChanges();

    // Mock the AuthService response to return an error
    authService.login.and.returnValue(of({ success: false, data: '' }));

    // Act: Call the onLogin method
    component.onLogin();

    // Assert: Check if the error snackbar is shown
    expect(snackBar.open).toHaveBeenCalledWith(
      'Login failed. Please check your credentials and try again.',
      'Close',
      { duration: 3000, panelClass: ['error-snackbar'] }
    );
  });

  it('should show error message if form is invalid on submit', () => {
    // Arrange: Set form to invalid state
    component.loginForm.controls['email'].setValue('');
    component.loginForm.controls['password'].setValue('');
    fixture.detectChanges();

    // Act: Call the onLogin method
    component.onLogin();

    // Assert: Check if the error snackbar is shown for invalid form
    expect(snackBar.open).toHaveBeenCalledWith(
      'Please fill in all fields correctly.',
      'Close',
      { duration: 3000, panelClass: ['error-snackbar'] }
    );
  });
});
