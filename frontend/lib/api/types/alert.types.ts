export interface AlertDto {
  id: number;
  type: string;
  value: number;
  threshold: number;
  createdAt: string;
  status: string;
}

export interface GetAlertsParams {
  status?: string;
  from?: string;
  to?: string;
  pageNumber?: number;
  pageSize?: number;
}
