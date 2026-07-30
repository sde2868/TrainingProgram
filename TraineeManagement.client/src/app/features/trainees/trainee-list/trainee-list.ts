import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { TraineeService } from '../../../core/services/trainee';
import { Trainee, TraineeStatus } from '../../../models/trainee.models';
import { Button } from '../../../shared/components/button/button';
import { ErrorMessage } from '../../../shared/components/error-message/error-message';
import { Loader } from '../../../shared/components/loader/loader';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { SelectInput, SelectOption } from '../../../shared/components/select-input/select-input';
import { StatusBadge } from '../../../shared/components/status-badge/status-badge';
import { TextInput } from '../../../shared/components/text-input/text-input';

@Component({
  selector: 'app-trainee-list',
  imports: [
    RouterLink,
    Button,
    ErrorMessage,
    Loader,
    Pagination,
    SelectInput,
    StatusBadge,
    TextInput
  ],
  templateUrl: './trainee-list.html',
  styleUrl: './trainee-list.css'
})
export class TraineeList implements OnInit {
  trainees = signal<Trainee[]>([]);

  search = signal('');
  status = signal<TraineeStatus | ''>('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalRecords = signal(0);

  isLoading = signal(false);
  errorMessage = signal('');

  readonly statusOptions: SelectOption[] = [
    { label: 'Active', value: 'Active' },
    { label: 'Busy', value: 'Busy' },
    { label: 'Offline', value: 'Offline' }
  ];

  readonly pageSizeOptions: SelectOption[] = [
    { label: '5', value: 5 },
    { label: '10', value: 10 },
    { label: '20', value: 20 }
  ];

  constructor(private readonly traineeService: TraineeService) {}

  ngOnInit(): void {
    this.loadTrainees();
  }

  loadTrainees(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.traineeService
      .getTrainees({
        pageNumber: this.pageNumber(),
        pageSize: Number(this.pageSize()),
        search: this.search().trim(),
        status: this.status()
      })
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
          console.log('Loading finished:', this.isLoading());
        })
      )
      .subscribe({
        next: (response) => {
          console.log('Trainee API response:', response);

          this.trainees.set(response.data ?? []);
          this.pageNumber.set(response.pageNumber ?? 1);
          this.pageSize.set(response.pageSize ?? 10);
          this.totalRecords.set(response.totalRecords ?? 0);
        },
        error: (error) => {
          console.error('Trainee API error:', error);

          if (error.status === 401) {
            this.errorMessage.set('You are not authorized. Please login again.');
            return;
          }

          if (error.status === 403) {
            this.errorMessage.set('You do not have permission to view trainees.');
            return;
          }

          this.errorMessage.set(
            error?.error?.message || 'Failed to load trainees. Please try again.'
          );
        }
      });
  }

  onSearchChanged(value: string): void {
    this.search.set(value);
  }

  onSearch(): void {
    this.pageNumber.set(1);
    this.loadTrainees();
  }

  onStatusChanged(value: string | number): void {
    this.status.set(value as TraineeStatus | '');
    this.pageNumber.set(1);
    this.loadTrainees();
  }

  onPageSizeChanged(value: string | number): void {
    this.pageSize.set(Number(value));
    this.pageNumber.set(1);
    this.loadTrainees();
  }

  onPageChanged(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
    this.loadTrainees();
  }

  deleteTrainee(id: number): void {
    const confirmed = confirm('Are you sure you want to delete this trainee?');

    if (!confirmed) {
      return;
    }

    this.traineeService.deleteTrainee(id).subscribe({
      next: () => {
        this.loadTrainees();
      },
      error: (error) => {
        this.errorMessage.set(
          error?.error?.message || 'Failed to delete trainee. Please try again.'
        );
      }
    });
  }
}