import { Component, EventEmitter, Input, Output } from '@angular/core';

type ButtonVariant = 'primary' | 'secondary' | 'danger';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.css'
})
export class Button {
  @Input() type: 'button' | 'submit' = 'button';
  @Input() variant: ButtonVariant = 'primary';
  @Input() disabled = false;
  @Input() label = '';
  @Input() fullWidth = false;

  @Output() clicked = new EventEmitter<void>();

  onClick(): void {
    if (this.disabled) {
      return;
    }
    this.clicked.emit();
  }
}