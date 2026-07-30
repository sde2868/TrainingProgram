import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextInput),
      multi: true
    }
  ]
})
export class TextInput implements ControlValueAccessor {
  @Input() id = '';
  @Input() label = '';
  @Input() name = '';
  @Input() type: 'text' | 'email' | 'password' | 'search' | 'date' | 'number' = 'text';
  @Input() placeholder = '';
  @Input() autocomplete = '';
  @Input() required = false;
  @Input() error = '';

  @Input() value = '';

  @Output() valueChange = new EventEmitter<string>();

  disabled = false;

  private propagateChange: (value: string) => void = () => {};
  private propagateTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value = value ?? '';
  }

  registerOnChange(fn: (value: string) => void): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.propagateTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInput(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.value = inputElement.value;

    this.propagateChange(this.value);
    this.valueChange.emit(this.value);
  }

  onBlur(): void {
    this.propagateTouched();
  }
}