import {
  TestCasePriority,
  TestCaseStatus,
} from '../../../core/models/common.models';

export interface CreateTestCaseRequest {
  name: string;
  description?: string;
  preconditions?: string;
  postconditions?: string;
  priority?: TestCasePriority;
  status?: TestCaseStatus;
}

export interface UpdateTestCaseRequest {
  name: string;
  description?: string;
  preconditions?: string;
  postconditions?: string;
  priority: TestCasePriority;
  status: TestCaseStatus;
}

export interface TestCaseResponse {
  id: string;
  suiteId: string;
  name: string;
  description?: string;
  preconditions?: string;
  postconditions?: string;
  priority: TestCasePriority;
  status: TestCaseStatus;
  createdAt: Date;
  updatedAt: Date;
}

export interface TestCaseDetailResponse extends TestCaseResponse {
  steps: Array<{
    id: string;
    testCaseId: string;
    stepOrder: number;
    action: string;
    expectedResult: string;
  }>;
}
