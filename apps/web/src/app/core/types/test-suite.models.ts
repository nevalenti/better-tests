export interface CreateTestSuiteRequest {
  name: string;
  description?: string;
}

export interface UpdateTestSuiteRequest {
  name: string;
  description?: string;
}

export interface TestSuiteResponse {
  id: string;
  projectId: string;
  name: string;
  description?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface TestSuiteDetailResponse extends TestSuiteResponse {
  testCases: Array<{
    id: string;
    suiteId: string;
    name: string;
    description?: string;
    preconditions?: string;
    postconditions?: string;
    priority: string;
    status: string;
    createdAt: Date;
    updatedAt: Date;
  }>;
}
