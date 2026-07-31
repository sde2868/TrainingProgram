import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { TraineeService } from '../../../core/services/trainee';
import { TraineeRequest, TraineeStatus } from '../../../models/trainee.models';
import { Button } from '../../../shared/components/button/button';
import { ErrorMessage } from '../../../shared/components/error-message/error-message';
import { Loader } from '../../../shared/components/loader/loader';
import { SelectInput, SelectOption } from '../../../shared/components/select-input/select-input';
import { TextInput } from '../../../shared/components/text-input/text-input';

@Component({
  selector: 'app-trainee-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    Button,
    ErrorMessage,
    Loader,
    SelectInput,
    TextInput
  ],
  templateUrl: './trainee-form.html',
  styleUrl: './trainee-form.css'
})
export class TraineeForm implements OnInit {
  isEditMode = signal(false);
  traineeId = signal<number | null>(null);

  isLoading = signal(false);
  isSaving = signal(false);
  pageError = signal('');

  fieldErrors = signal<Record<string, string>>({});

  readonly statusOptions: SelectOption[] = [
    { label: 'Active', value: 'Active' },
    { label: 'Busy', value: 'Busy' },
    { label: 'Offline', value: 'Offline' }
  ];

  private readonly fb = inject(FormBuilder);
  private readonly traineeService = inject(TraineeService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group({
    firstName: [
      '',
      [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        Validators.pattern(/^[a-zA-Z]+$/)
      ]
    ],
    lastName: [
      '',
      [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        Validators.pattern(/^[a-zA-Z]+$/)
      ]
    ],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
    techStack: ['', [Validators.required]],
    status: ['', [Validators.required]]
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id || Number.isNaN(id)) {
      return;
    }

    console.log('Edit route id:', id);

    this.isEditMode.set(true);
    this.traineeId.set(id);
    this.loadTrainee(id);
  }

  loadTrainee(id: number): void {
    this.isLoading.set(true);
    this.pageError.set('');

    this.traineeService
      .getTraineeById(id)
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        })
      )
      .subscribe({
        next: (trainee) => {
          console.log('Loaded trainee:', trainee);

          this.form.patchValue({
            firstName: trainee.firstName,
            lastName: trainee.lastName,
            email: trainee.email,
            techStack: trainee.techStack?.join(', ') ?? '',
            status: String(trainee.status)
          });

          console.log('Form after patch:', JSON.stringify(this.form.getRawValue()));
        },
        error: (error) => {
          this.pageError.set(
            error?.error?.message || 'Failed to load trainee details.'
          );
        }
      });
  }

  submit(): void {
    this.pageError.set('');
    this.fieldErrors.set({});

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = this.buildRequest();

    this.isSaving.set(true);

    const saveRequest =
      this.isEditMode() && this.traineeId()
        ? this.traineeService.updateTrainee(this.traineeId()!, request)
        : this.traineeService.createTrainee(request);

    saveRequest.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.router.navigateByUrl('/trainees');
      },
      error: (error) => {
        this.isSaving.set(false);
        this.handleBackendErrors(error);
      }
    });
  }

  getControlError(controlName: string): string {
    const backendError = this.fieldErrors()[controlName];

    if (backendError) {
      return backendError;
    }

    const control = this.form.get(controlName);

    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      return 'This field is required.';
    }

    if (control.errors['email']) {
      return 'Enter a valid email address.';
    }

    if (control.errors['minlength']) {
      return `Minimum length is ${control.errors['minlength'].requiredLength}.`;
    }

    if (control.errors['maxlength']) {
      return `Maximum length is ${control.errors['maxlength'].requiredLength}.`;
    }

    if (control.errors['pattern']) {
      return 'Only letters are allowed.';
    }

    return 'Invalid value.';
  }

  private buildRequest(): TraineeRequest {
    const raw = this.form.getRawValue();

    return {
      firstName: raw.firstName.trim(),
      lastName: raw.lastName.trim(),
      email: raw.email.trim(),
      techStack: raw.techStack
        .split(',')
        .map((item) => item.trim())
        .filter(Boolean),
      status: raw.status as TraineeStatus
    };
  }

  private handleBackendErrors(error: any): void {
    const backendErrors = error?.error?.errors;

    if (backendErrors) {
      const mappedErrors: Record<string, string> = {};

      for (const key of Object.keys(backendErrors)) {
        const normalizedKey = this.normalizeBackendFieldName(key);
        mappedErrors[normalizedKey] = backendErrors[key][0];
      }

      this.fieldErrors.set(mappedErrors);
      return;
    }

    this.pageError.set(
      error?.error?.message || 'Failed to save trainee. Please try again.'
    );
  }

  private normalizeBackendFieldName(key: string): string {
    const cleanKey = key.replace('$.', '');

    return cleanKey.charAt(0).toLowerCase() + cleanKey.slice(1);
  }
}