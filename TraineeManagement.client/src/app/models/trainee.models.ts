export type TraineeStatus = 'Active' | 'Busy' | 'Offline';

export interface Trainee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  techStack: string[];
  status: TraineeStatus;
  createdAt: string;
  updatedAt: string;
}

export interface TraineeRequest {
  firstName: string;
  lastName: string;
  email: string;
  techStack: string[];
  status: TraineeStatus;
}