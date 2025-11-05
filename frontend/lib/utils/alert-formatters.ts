import type { AlertDto } from "@/lib/api/types/alert.types";

/**
 * Alert type constants
 */
export type AlertType = "Temperature" | "Humidity";

/**
 * Alert status constants
 */
export type AlertStatus = "Open" | "Acknowledged" | "Resolved";

/**
 * Get the unit symbol for an alert type
 */
export function getAlertUnit(type: string): string {
  return type.toLowerCase() === "temperature" ? "°C" : "%";
}

/**
 * Format alert value with appropriate unit
 */
export function formatAlertValue(alert: AlertDto, precision: number = 2): string {
  const unit = getAlertUnit(alert.type);
  return `${alert.value.toFixed(precision)}${unit}`;
}

/**
 * Format threshold value with appropriate unit
 */
export function formatThresholdValue(alert: AlertDto, precision: number = 2): string {
  const unit = getAlertUnit(alert.type);
  return `${alert.threshold.toFixed(precision)}${unit}`;
}

/**
 * Format alert comparison (value vs threshold)
 */
export function formatAlertComparison(alert: AlertDto, precision: number = 1): string {
  const unit = getAlertUnit(alert.type);
  return `${alert.value.toFixed(precision)} > ${alert.threshold.toFixed(precision)}${unit}`;
}

/**
 * Get CSS classes for alert status badge
 */
export function getStatusBadgeClasses(status: string): string {
  switch (status.toLowerCase()) {
    case "open":
      return "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200";
    case "acknowledged":
      return "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200";
    case "resolved":
      return "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200";
    default:
      return "bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200";
  }
}

/**
 * Get CSS classes for alert type badge
 */
export function getTypeBadgeClasses(type: string): string {
  switch (type.toLowerCase()) {
    case "temperature":
      return "bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200";
    case "humidity":
      return "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200";
    default:
      return "bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200";
  }
}

/**
 * Check if alert type is valid
 */
export function isValidAlertType(type: string): type is AlertType {
  const normalized = type.toLowerCase();
  return normalized === "temperature" || normalized === "humidity";
}

/**
 * Check if alert status is valid
 */
export function isValidAlertStatus(status: string): status is AlertStatus {
  const normalized = status.toLowerCase();
  return normalized === "open" || normalized === "acknowledged" || normalized === "resolved";
}
