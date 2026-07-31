import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  forwardRef,
  Input,
  Output,
  inject
} from '@angular/core';
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
  private readonly cdr = inject(ChangeDetectorRef);

  @Input() id = '';
  @Input() name = '';
  @Input() label = '';
  @Input() placeholder = 'Select';
  @Input() options: SelectOption[] = [];
  @Input() error = '';

  @Input()
  set value(value: string | number | null) {
    this.displayValue = value === null || value === undefined ? '' : String(value);
  }

  get value(): string {
    return this.displayValue;
  }

  @Output() valueChange = new EventEmitter<string>();

  displayValue = '';
  disabled = false;

  private propagateChange: (value: string) => void = () => {};
  private propagateTouched: () => void = () => {};

  writeValue(value: string | number | null): void {
    this.displayValue = value === null || value === undefined ? '' : String(value);
    this.cdr.markForCheck();
  }

  registerOnChange(fn: (value: string) => void): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.propagateTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
    this.cdr.markForCheck();
  }

  onSelectChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.displayValue = selectElement.value;

    this.propagateChange(this.displayValue);
    this.valueChange.emit(this.displayValue);
  }

  onBlur(): void {
    this.propagateTouched();
  }

  isSelected(optionValue: string | number): boolean {
    return String(optionValue) === this.displayValue;
  }
}