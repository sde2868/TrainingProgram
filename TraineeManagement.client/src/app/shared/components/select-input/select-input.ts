import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

export interface SelectOption {
  label: string;
  value: string | number;
}

@Component({
  selector: 'app-select-input',
  imports: [FormsModule],
  templateUrl: './select-input.html',
  styleUrl: './select-input.css'
})
export class SelectInput {
  @Input() id = '';
  @Input() name = '';
  @Input() label = '';
  @Input() value: string | number = '';
  @Input() options: SelectOption[] = [];
  @Input() placeholder = 'Select';
  @Input() error = '';

  @Output() valueChange = new EventEmitter<string | number>();

  onValueChange(value: string | number): void {
    this.valueChange.emit(value);
  }
}