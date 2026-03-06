export interface CreateProjectRequest {
  name: string;
  description?: string;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string;
}

export interface ProjectResponse {
  id: string;
  name: string;
  description?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface ProjectDetailResponse extends ProjectResponse {
  testSuites: Array<{
    id: string;
    projectId: string;
    name: string;
    description?: string;
    createdAt: Date;
    updatedAt: Date;
  }>;
}
