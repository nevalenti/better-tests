import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiHttpClient } from '../../../core/http-client';
import { PaginatedResponse } from '../../../core/models/common.models';
import {
  CreateTestCaseStepRequest,
  TestCaseStepResponse,
  UpdateTestCaseStepRequest,
} from '../models/test-case-step.models';

@Injectable({
  providedIn: 'root',
})
export class TestCaseStepService {
  private httpClient = inject(ApiHttpClient);

  getTestCaseSteps(
    caseId: string,
    page = 1,
    pageSize = 20,
  ): Observable<PaginatedResponse<TestCaseStepResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.httpClient.get(`/api/v1/cases/${caseId}/steps`, { params });
  }

  getTestCaseStep(
    caseId: string,
    stepId: string,
  ): Observable<TestCaseStepResponse> {
    return this.httpClient.get(`/api/v1/cases/${caseId}/steps/${stepId}`);
  }

  createTestCaseStep(
    caseId: string,
    request: CreateTestCaseStepRequest,
  ): Observable<TestCaseStepResponse> {
    return this.httpClient.post(`/api/v1/cases/${caseId}/steps`, request);
  }

  updateTestCaseStep(
    caseId: string,
    stepId: string,
    request: UpdateTestCaseStepRequest,
  ): Observable<void> {
    return this.httpClient.put(
      `/api/v1/cases/${caseId}/steps/${stepId}`,
      request,
    );
  }

  deleteTestCaseStep(caseId: string, stepId: string): Observable<void> {
    return this.httpClient.delete(`/api/v1/cases/${caseId}/steps/${stepId}`);
  }
}
