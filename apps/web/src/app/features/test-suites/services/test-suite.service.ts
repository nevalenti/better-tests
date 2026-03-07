import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiHttpClient } from '../../../core/http-client';
import { PaginatedResponse } from '../../../core/models/common.models';
import {
  CreateTestSuiteRequest,
  TestSuiteDetailResponse,
  TestSuiteResponse,
  UpdateTestSuiteRequest,
} from '../models/test-suite.models';

@Injectable({
  providedIn: 'root',
})
export class TestSuiteService {
  private httpClient = inject(ApiHttpClient);

  getTestSuites(
    projectId: string,
    page = 1,
    pageSize = 20,
  ): Observable<PaginatedResponse<TestSuiteResponse>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.httpClient.get(`/api/v1/projects/${projectId}/suites`, {
      params,
    });
  }

  getTestSuite(
    projectId: string,
    suiteId: string,
  ): Observable<TestSuiteDetailResponse> {
    return this.httpClient.get(
      `/api/v1/projects/${projectId}/suites/${suiteId}`,
    );
  }

  createTestSuite(
    projectId: string,
    request: CreateTestSuiteRequest,
  ): Observable<TestSuiteResponse> {
    return this.httpClient.post(
      `/api/v1/projects/${projectId}/suites`,
      request,
    );
  }

  updateTestSuite(
    projectId: string,
    suiteId: string,
    request: UpdateTestSuiteRequest,
  ): Observable<void> {
    return this.httpClient.put(
      `/api/v1/projects/${projectId}/suites/${suiteId}`,
      request,
    );
  }

  deleteTestSuite(projectId: string, suiteId: string): Observable<void> {
    return this.httpClient.delete(
      `/api/v1/projects/${projectId}/suites/${suiteId}`,
    );
  }
}
