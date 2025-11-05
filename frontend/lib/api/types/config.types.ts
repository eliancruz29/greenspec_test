export interface ConfigDto {
  id: number;
  tempMax: number;
  humidityMax: number;
  updatedAt: string;
}

export interface UpdateConfigRequest {
  tempMax: number;
  humidityMax: number;
}
