import { inject, Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiHttpClient } from '../../../core/http-client';
import { PaginatedResponse } from '../../../core/models/common.models';
import {
  CreateProjectRequest,
  ProjectResponse,
} from '../models/project.models';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private httpClient = inject(ApiHttpClient);

  getProjects(): Observable<PaginatedResponse<ProjectResponse>> {
    return this.httpClient.get('/api/v1/projects');
  }

  createProject(
    name: string,
    description?: string,
  ): Observable<ProjectResponse> {
    const request: CreateProjectRequest = { name, description };
    return this.httpClient.post('/api/v1/projects', request);
  }
}
