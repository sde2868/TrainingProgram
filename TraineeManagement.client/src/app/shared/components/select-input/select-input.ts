import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface SelectOption {
  label: string;
  value: string | number;
}

@Component({
  selector: 'app-select-input',
  imports: [],
  templateUrl: './select-input.html',
  styleUrl: './select-input.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectInput),
      multi: true
    }
  ]
})
export class SelectInput implements ControlValueAccessor {
  @Input() id = '';
  @Input() name = '';
  @Input() label = '';
  @Input() placeholder = 'Select';
  @Input() options: SelectOption[] = [];
  @Input() error = '';

  @Input() value: string | number = '';

  @Output() valueChange = new EventEmitter<string | number>();

  disabled = false;

  private propagateChange: (value: string | number) => void = () => {};
  private propagateTouched: () => void = () => {};

  writeValue(value: string | number | null): void {
    this.value = value ?? '';
  }

  registerOnChange(fn: (value: string | number) => void): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.propagateTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onSelectChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.value = selectElement.value;

    this.propagateChange(this.value);
    this.valueChange.emit(this.value);
  }

  onBlur(): void {
    this.propagateTouched();
  }
}