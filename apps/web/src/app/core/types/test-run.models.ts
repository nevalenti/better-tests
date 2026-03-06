import { TestRunStatus } from './common.models';

export interface CreateTestRunRequest {
  name: string;
  environment: string;
  startedAt?: Date;
  completedAt?: Date;
  status?: TestRunStatus;
}

export interface UpdateTestRunRequest {
  name: string;
  environment: string;
  startedAt?: Date;
  completedAt?: Date;
  status: TestRunStatus;
}

export interface TestRunResponse {
  id: string;
  projectId: string;
  name: string;
  environment: string;
  executedBy: string;
  startedAt?: Date;
  completedAt?: Date;
  status: TestRunStatus;
}
