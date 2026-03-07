export interface CreateTestCaseStepRequest {
  stepOrder: number;
  action: string;
  expectedResult: string;
}

export interface UpdateTestCaseStepRequest {
  stepOrder: number;
  action: string;
  expectedResult: string;
}

export interface TestCaseStepResponse {
  id: string;
  testCaseId: string;
  stepOrder: number;
  action: string;
  expectedResult: string;
}
