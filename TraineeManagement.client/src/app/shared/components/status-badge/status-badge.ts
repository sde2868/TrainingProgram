import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  imports: [],
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.css'
})
export class StatusBadge {
  @Input() status = '';
}