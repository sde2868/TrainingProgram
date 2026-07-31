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
  private readonly cdr = inject(ChangeDetectorRef);

  @Input() id = '';
  @Input() label = '';
  @Input() name = '';
  @Input() type: 'text' | 'email' | 'password' | 'search' | 'date' | 'number' = 'text';
  @Input() placeholder = '';
  @Input() autocomplete = '';
  @Input() required = false;
  @Input() error = '';

  @Input()
  set value(value: string | null) {
    this.displayValue = value ?? '';
  }

  get value(): string {
    return this.displayValue;
  }

  @Output() valueChange = new EventEmitter<string>();

  displayValue = '';
  disabled = false;

  private propagateChange: (value: string) => void = () => {};
  private propagateTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.displayValue = value ?? '';
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

  onInput(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.displayValue = inputElement.value;

    this.propagateChange(this.displayValue);
    this.valueChange.emit(this.displayValue);
  }

  onBlur(): void {
    this.propagateTouched();
  }
}