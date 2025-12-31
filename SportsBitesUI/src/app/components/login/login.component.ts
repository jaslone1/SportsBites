import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Optional, but good for *ngIf etc.
import { FormsModule } from '@angular/forms';   // <--- 1. ADD THIS IMPORT
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html'
})

export class LoginComponent {
  loginData = { email: '', password: '' };
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  onLogin() {
    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        console.log('Login successful!', response);
        this.router.navigate(['/events']); // Redirect to your main list
      },
      error: (err) => {
        this.errorMessage = 'Invalid email or password. Please try again.';
        console.error('Login error:', err);
      }
    });
  }
}
