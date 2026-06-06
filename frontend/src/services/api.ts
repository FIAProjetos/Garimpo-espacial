import type {
  AlertDto,
  AuthResponse,
  ClusterDto,
  ClusteringRequest,
  ClusteringResultDto,
  DebrisDto,
  LoginRequest,
  PagedResult,
  RegisterRequest,
  UserDto,
} from '../types/api';
import { getToken } from './authStorage';

const API_URL = process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:8080';

export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

async function request<T>(
  path: string,
  options: RequestInit = {},
  auth = true,
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  };

  if (auth) {
    const token = await getToken();
    if (token) headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_URL}${path}`, { ...options, headers });

  if (!response.ok) {
    const body = await response.text();
    throw new ApiError(body || response.statusText, response.status);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export function login(data: LoginRequest): Promise<AuthResponse> {
  return request<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(data),
  }, false);
}

export function register(data: RegisterRequest): Promise<UserDto> {
  return request<UserDto>('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(data),
  }, false);
}

export function getDebrisPage(page = 1, pageSize = 20): Promise<PagedResult<DebrisDto>> {
  return request(`/api/debris?page=${page}&pageSize=${pageSize}`);
}

export function getClustersPage(page = 1, pageSize = 20): Promise<PagedResult<ClusterDto>> {
  return request(`/api/clusters?page=${page}&pageSize=${pageSize}`);
}

export function getAlerts(): Promise<AlertDto[]> {
  return request('/api/alerts');
}

export async function fetchAllDebrisForChart(maxItems = 300): Promise<DebrisDto[]> {
  const pageSize = 100;
  const items: DebrisDto[] = [];
  let page = 1;
  let hasNext = true;

  while (hasNext && items.length < maxItems) {
    const result = await getDebrisPage(page, pageSize);
    items.push(...result.items);
    hasNext = result.hasNextPage;
    page += 1;
  }

  return items.slice(0, maxItems);
}

export async function fetchAllClustersForChart(): Promise<ClusterDto[]> {
  const result = await getClustersPage(1, 100);
  return result.items;
}

export function runIngestion(): Promise<unknown> {
  return request('/api/ingestion?group=cosmos-2251-debris', { method: 'POST' });
}

export function runClustering(params: ClusteringRequest): Promise<ClusteringResultDto> {
  return request<ClusteringResultDto>('/api/clusters/run', {
    method: 'POST',
    body: JSON.stringify(params),
  });
}
