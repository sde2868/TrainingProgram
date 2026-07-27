import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [FormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css'
})
export class TextInput {
  @Input() id = '';
  @Input() label = '';
  @Input() name = '';
  @Input() type: 'text' | 'email' | 'password' | 'search' | 'date' | 'number' = 'text';
  @Input() placeholder = '';
  @Input() value = '';
  @Input() error = '';
  @Input() autocomplete = '';
  @Input() required = false;

  @Output() valueChange = new EventEmitter<string>();

  onValueChange(value: string): void {
    this.valueChange.emit(value);
  }
}