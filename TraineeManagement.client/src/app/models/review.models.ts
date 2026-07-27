export type ReviewStatus = 'Accepted' | 'ChangesRequired' | 'Rejected';

export interface Review {
  id: number;
  taskSubmissionId: number;
  mentorId: number;
  feedback: string;
  score: number;
  status: ReviewStatus;
  reviewedDate: string;
}

export interface ReviewRequest {
  taskSubmissionId: number;
  mentorId: number;
  feedback: string;
  score: number;
  status: ReviewStatus;
  reviewedDate: string;
}