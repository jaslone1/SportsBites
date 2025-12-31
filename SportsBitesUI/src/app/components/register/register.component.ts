import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  model: any = {
    email: '',
    password: '',
    confirmPassword: ''
  };

  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    if (this.model.password !== this.model.confirmPassword) {
      this.errorMessage = "Passwords do not match.";
      return;
    }

    this.authService.register(this.model).subscribe({
      next: (response) => {
        console.log('Registration success', response);
        alert('Registration successful! Please login.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Registration error', err);
        // Identity returns errors in an array: [{code: "...", description: "..."}]
        if (err.error && Array.isArray(err.error)) {
          this.errorMessage = err.error.map((e: any) => e.description).join(' ');
        } else {
          this.errorMessage = "Registration failed. Please try again.";
        }
      }
    });
  }
}
