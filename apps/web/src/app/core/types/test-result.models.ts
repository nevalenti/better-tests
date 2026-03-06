import { TestResultStatus } from './common.models';

export interface CreateTestResultRequest {
  testCaseId?: string;
  result: TestResultStatus;
  comments?: string;
  defectLink?: string;
}

export interface UpdateTestResultRequest {
  result: TestResultStatus;
  comments?: string;
  defectLink?: string;
}

export interface TestResultResponse {
  id: string;
  testRunId: string;
  testCaseId?: string;
  result: TestResultStatus;
  comments?: string;
  defectLink?: string;
  executedAt: Date;
  executedBy: string;
}
