export interface PaginatedResponse<T> {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  data: T[];
}

export interface ApiErrorResponse {
  success?: boolean;
  statusCode?: number;
  message?: string;
  details?: string | null;
  errors?: Record<string, string[]>;
}