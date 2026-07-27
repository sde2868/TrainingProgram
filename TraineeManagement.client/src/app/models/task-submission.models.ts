export type TaskSubmissionStatus = 'Submitted' | 'Resubmitted';

export interface TaskSubmission {
  id: number;
  taskAssignmentId: number;
  submissionUrl: string;
  notes: string;
  submittedDate: string;
  status: TaskSubmissionStatus;
}

export interface TaskSubmissionRequest {
  taskAssignmentId: number;
  submissionUrl: string;
  notes: string;
  submittedDate: string;
  status: TaskSubmissionStatus;
}