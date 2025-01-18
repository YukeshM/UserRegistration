import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http'; // Added HttpClientModule import
import { RegistrationComponent } from './registration.component';
import { AuthService } from '../../service/auth.service';
import { of, throwError } from 'rxjs';

describe('RegistrationComponent', () => {
  let component: RegistrationComponent;
  let fixture: ComponentFixture<RegistrationComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    const authServiceMock = jasmine.createSpyObj('AuthService', ['register']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);
    const snackBarMock = jasmine.createSpyObj('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      imports: [
        RegistrationComponent, // Import standalone component
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatButtonModule,
        MatIconModule,
        BrowserAnimationsModule,
        HttpClientModule, // HttpClientModule imported
      ],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: MatSnackBar, useValue: snackBarMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegistrationComponent);
    component = fixture.componentInstance;

    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    snackBarSpy = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with empty controls', () => {
    const form = component.registerForm;
    expect(form.get('userName')?.value).toBe('');
    expect(form.get('email')?.value).toBe('');
    expect(form.get('password')?.value).toBe('');
    expect(form.get('registrationDate')?.value).toBe('');
  });

  it('should mark the form invalid if required fields are missing', () => {
    component.registerForm.patchValue({
      userName: '',
      email: '',
      password: '',
      registrationDate: '',
    });
    expect(component.registerForm.valid).toBeFalse();
  });

  it('should call AuthService.register on valid form submission', () => {
    authServiceSpy.register.and.returnValue(of({ success: true }));

    component.registerForm.patchValue({
      userName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      password: 'Password1!',
      registrationDate: '2025-01-18',
    });
    component.onRegister();

    expect(authServiceSpy.register).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Registration successful! Redirecting...',
      'Close',
      jasmine.objectContaining({ duration: 3000 })
    );
  });

  it('should show an error snackbar on registration failure', () => {
    authServiceSpy.register.and.returnValue(
      throwError(() => new Error('Error'))
    );

    component.registerForm.patchValue({
      userName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      password: 'Password1!',
      registrationDate: '2025-01-18',
    });
    component.onRegister();

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Registration failed. Please contact administrator.',
      'Close',
      jasmine.objectContaining({ duration: 3000 })
    );
  });

  it('should handle file upload and validate size', () => {
    const mockFile = new File(['content'], 'test-file.jpg', {
      type: 'image/jpeg',
    });
    Object.defineProperty(mockFile, 'size', { value: 1024 }); // Setting mock size
    const event = { target: { files: [mockFile] } } as any;

    component.onFileSelected(event);

    expect(component.registerForm.get('document')?.value).toBe(mockFile);
  });

  it('should show an error if file exceeds the size limit', () => {
    const oversizedFile = new File(['content'], 'large-file.jpg', {
      type: 'image/jpeg',
    });
    Object.defineProperty(oversizedFile, 'size', { value: 15 * 1024 * 1024 }); // Setting mock oversized size (15 MB)
    const event = { target: { files: [oversizedFile] } } as any;

    component.onFileSelected(event);

    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'File size should not exceed 10MB.',
      'Close',
      jasmine.objectContaining({ duration: 3000 })
    );
  });

  it('should toggle password visibility', () => {
    expect(component.hidePassword).toBeTrue();
    component.togglePasswordVisibility();
    expect(component.hidePassword).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.hidePassword).toBeTrue();
  });
});
