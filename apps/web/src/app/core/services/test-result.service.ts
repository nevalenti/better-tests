import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiHttpClient } from '../http-client';
import { PaginatedResponse, TestResultStatus } from '../types/common.models';
import {
  CreateTestResultRequest,
  TestResultResponse,
  UpdateTestResultRequest,
} from '../types/test-result.models';

@Injectable({
  providedIn: 'root',
})
export class TestResultService {
  private httpClient = inject(ApiHttpClient);

  getTestResults(
    runId: string,
    page = 1,
    pageSize = 20,
    status?: TestResultStatus,
  ): Observable<PaginatedResponse<TestResultResponse>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }

    return this.httpClient.get(`/api/v1/runs/${runId}/results`, { params });
  }

  getTestResult(
    runId: string,
    resultId: string,
  ): Observable<TestResultResponse> {
    return this.httpClient.get(`/api/v1/runs/${runId}/results/${resultId}`);
  }

  createTestResult(
    runId: string,
    request: CreateTestResultRequest,
  ): Observable<TestResultResponse> {
    return this.httpClient.post(`/api/v1/runs/${runId}/results`, request);
  }

  updateTestResult(
    runId: string,
    resultId: string,
    request: UpdateTestResultRequest,
  ): Observable<void> {
    return this.httpClient.put(
      `/api/v1/runs/${runId}/results/${resultId}`,
      request,
    );
  }

  deleteTestResult(runId: string, resultId: string): Observable<void> {
    return this.httpClient.delete(`/api/v1/runs/${runId}/results/${resultId}`);
  }
}
