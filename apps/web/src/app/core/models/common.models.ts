export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export enum TestCasePriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical',
}

export enum TestCaseStatus {
  Draft = 'Draft',
  Active = 'Active',
  Deprecated = 'Deprecated',
}

export enum TestRunStatus {
  InProgress = 'InProgress',
  Completed = 'Completed',
  Paused = 'Paused',
}

export enum TestResultStatus {
  Passed = 'Passed',
  Failed = 'Failed',
  Skipped = 'Skipped',
  Blocked = 'Blocked',
}
