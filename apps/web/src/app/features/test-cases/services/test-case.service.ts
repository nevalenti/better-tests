import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiHttpClient } from '../../../core/http-client';
import {
  PaginatedResponse,
  TestCasePriority,
  TestCaseStatus,
} from '../../../core/models/common.models';
import {
  CreateTestCaseRequest,
  TestCaseDetailResponse,
  TestCaseResponse,
  UpdateTestCaseRequest,
} from '../models/test-case.models';

@Injectable({
  providedIn: 'root',
})
export class TestCaseService {
  private httpClient = inject(ApiHttpClient);

  getTestCases(
    suiteId: string,
    page = 1,
    pageSize = 20,
    priority?: TestCasePriority,
    status?: TestCaseStatus,
  ): Observable<PaginatedResponse<TestCaseResponse>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (priority) {
      params = params.set('priority', priority);
    }
    if (status) {
      params = params.set('status', status);
    }

    return this.httpClient.get(`/api/v1/suites/${suiteId}/cases`, { params });
  }

  getTestCase(
    suiteId: string,
    caseId: string,
  ): Observable<TestCaseDetailResponse> {
    return this.httpClient.get(`/api/v1/suites/${suiteId}/cases/${caseId}`);
  }

  createTestCase(
    suiteId: string,
    request: CreateTestCaseRequest,
  ): Observable<TestCaseResponse> {
    return this.httpClient.post(`/api/v1/suites/${suiteId}/cases`, request);
  }

  updateTestCase(
    suiteId: string,
    caseId: string,
    request: UpdateTestCaseRequest,
  ): Observable<void> {
    return this.httpClient.put(
      `/api/v1/suites/${suiteId}/cases/${caseId}`,
      request,
    );
  }

  deleteTestCase(suiteId: string, caseId: string): Observable<void> {
    return this.httpClient.delete(`/api/v1/suites/${suiteId}/cases/${caseId}`);
  }
}
