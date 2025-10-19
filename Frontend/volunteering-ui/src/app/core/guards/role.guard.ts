import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const militaryGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const user = authService.currentUser();
  
  if (user && user.role === 'MILITARY') {
    return true;
  }
  
  return router.parseUrl('/dashboard');
};

export const volunteerGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const user = authService.currentUser();
  
  if (user && user.role === 'VOLUNTEER') {
    return true;
  }
  
  return router.parseUrl('/dashboard');
};