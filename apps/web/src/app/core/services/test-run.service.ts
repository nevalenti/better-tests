import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiHttpClient } from '../http-client';
import { PaginatedResponse, TestRunStatus } from '../types/common.models';
import {
  CreateTestRunRequest,
  TestRunResponse,
  UpdateTestRunRequest,
} from '../types/test-run.models';

@Injectable({
  providedIn: 'root',
})
export class TestRunService {
  private httpClient = inject(ApiHttpClient);

  getTestRuns(
    projectId: string,
    page = 1,
    pageSize = 20,
    status?: TestRunStatus,
  ): Observable<PaginatedResponse<TestRunResponse>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }

    return this.httpClient.get(`/api/v1/projects/${projectId}/runs`, {
      params,
    });
  }

  getTestRun(projectId: string, runId: string): Observable<TestRunResponse> {
    return this.httpClient.get(`/api/v1/projects/${projectId}/runs/${runId}`);
  }

  createTestRun(
    projectId: string,
    request: CreateTestRunRequest,
  ): Observable<TestRunResponse> {
    return this.httpClient.post(`/api/v1/projects/${projectId}/runs`, request);
  }

  updateTestRun(
    projectId: string,
    runId: string,
    request: UpdateTestRunRequest,
  ): Observable<void> {
    return this.httpClient.put(
      `/api/v1/projects/${projectId}/runs/${runId}`,
      request,
    );
  }

  deleteTestRun(projectId: string, runId: string): Observable<void> {
    return this.httpClient.delete(
      `/api/v1/projects/${projectId}/runs/${runId}`,
    );
  }
}
