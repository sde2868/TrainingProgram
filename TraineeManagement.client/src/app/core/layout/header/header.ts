import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { Auth } from '../../auth/auth';
import { Button } from '../../../shared/components/button/button';

@Component({
  selector: 'app-header',
  imports: [Button],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  constructor(
    private readonly auth: Auth,
    private readonly router: Router
  ) {}

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}