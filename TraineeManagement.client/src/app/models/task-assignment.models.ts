export type LearningTaskStatus = 'Draft' | 'Published' | 'Closed';

export interface LearningTask {
  id: number;
  title: string;
  description: string;
  expectedTechStack: string[];
  status: LearningTaskStatus;
  dueDate: string;
  createdAt: string;
  updatedAt: string;
}

export interface LearningTaskRequest {
  title: string;
  description: string;
  expectedTechStack: string[];
  status: LearningTaskStatus;
  dueDate: string;
}