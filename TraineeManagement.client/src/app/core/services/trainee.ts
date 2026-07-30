import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../../core/services/api-config';
import { PaginatedResponse } from '../../models/api.models';
import {
  Trainee,
  TraineeRequest,
  TraineeStatus
} from '../../models/trainee.models';

@Injectable({
  providedIn: 'root'
})
export class TraineeService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/trainees`;

  constructor(private readonly http: HttpClient) { }

  getTrainees(options: {
    pageNumber: number;
    pageSize: number;
    search?: string;
    status?: TraineeStatus | '';
  }): Observable<PaginatedResponse<Trainee>> {
    let params = new HttpParams()
      .set('pageNumber', options.pageNumber)
      .set('pageSize', options.pageSize);

    if (options.search) {
      params = params.set('search', options.search);
    }

    if (options.status) {
      params = params.set('status', options.status);
    }

    return this.http.get<PaginatedResponse<Trainee>>(this.baseUrl, { params });
  }

  getTraineeById(id: number): Observable<Trainee> {
    return this.http.get<Trainee>(`${this.baseUrl}/${id}`);
  }

  createTrainee(request: TraineeRequest): Observable<Trainee> {
    return this.http.post<Trainee>(this.baseUrl, request);
  }

  updateTrainee(id: number, request: TraineeRequest): Observable<Trainee> {
    return this.http.put<Trainee>(`${this.baseUrl}/${id}`, request);
  }

  deleteTrainee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}