import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html'
})

export class LoginComponent implements OnInit {
  loginData = {
    email: '',
    password: ''
  };
  infoMessage: string | null = null;
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['message'] === 'login_required') {
        this.infoMessage = "You need to be logged in to view event details or join a party!";
      }
    });
  }

  onLogin() {
    this.authService.login(this.loginData).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        this.errorMessage = "Invalid email or password.";
      }
    });
  }
}
