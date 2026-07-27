export type MentorStatus = 'Active' | 'Inactive';

export interface Mentor {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  expertise: string[];
  status: MentorStatus;
  createdAt: string;
  updatedAt: string;
}

export interface MentorRequest {
  firstName: string;
  lastName: string;
  email: string;
  expertise: string[];
  status: MentorStatus;
}