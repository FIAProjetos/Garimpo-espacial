export type UserDto = {
  id: string;
  email: string;
  fullName: string;
  role: string;
  createdAt: string;
};

export type AuthResponse = {
  token: string;
  expiresAt: string;
  user: UserDto;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  email: string;
  password: string;
  fullName: string;
};

export type DebrisDto = {
  id: string;
  noradId: number;
  name: string;
  inclinationDegrees: number;
  eccentricity: number;
  meanMotionRevsPerDay: number;
  altitudeKm: number;
  classification: string;
  capturedAt: string;
  clusterId: string | null;
};

export type ClusterDto = {
  id: string;
  label: number;
  centroidAltitudeKm: number;
  centroidInclinationDegrees: number;
  memberCount: number;
  density: number;
  createdAt: string;
};

export type AlertDto = {
  id: string;
  alertType: string;
  severity: string;
  message: string;
  requiresImmediateAction: boolean;
  isAcknowledged: boolean;
  triggeredAt: string;
  acknowledgedAt: string | null;
};

export type ClusteringRequest = {
  epsilon: number;
  minPoints: number;
};

export type ClusteringResultDto = {
  processedDebris: number;
  clustersFound: number;
  noiseCount: number;
  epsilon: number;
  minPoints: number;
  completedAt: string;
  alertsGenerated: number;
};

export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
};
