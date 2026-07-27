export interface SubmissionFile {
  id: number;
  submissionId: number;
  originalFileName: string;
  contentType: string;
  size: number;
  checksum: string;
  uploadedByUserId?: number | null;
  createdAt: string;
  updatedAt: string;
}

export interface SubmissionQueuedResponse {
  taskSubmissionId: number;
  submissionFileId: number;
  messageId: string;
  status: 'Queued';
}