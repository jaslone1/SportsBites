import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html'
})

export class LoginComponent implements OnInit {
  model: any = {};
  infoMessage: string | null = null;

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

  onSubmit() {
    this.authService.login(this.model).subscribe({
      next: () => {
        // After login, check if there is a returnUrl, otherwise go home
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        console.error(err);
        this.infoMessage = "Invalid login attempt.";
      }
    });
  }
}
