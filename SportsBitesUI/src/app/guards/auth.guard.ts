import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  } else {
    this.router.navigate(['/login'], {
      queryParams: {
        message: 'login_required',
        returnUrl: state.url
  }
};
    return false;
  }
};

