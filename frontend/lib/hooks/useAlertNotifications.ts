import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import type { AlertDto } from "@/lib/api/types/alert.types";
import { alertHub } from "@/lib/signalr";
import { queryKeys } from "@/lib/api/query-keys";
import { getToken } from "@/lib/storage/auth-storage";
import { useBrowserNotifications } from "./useBrowserNotifications";
import { formatAlertComparison } from "@/lib/utils/alert-formatters";

/**
 * Custom hook for managing real-time alert notifications via SignalR
 *
 * Handles:
 * - SignalR connection lifecycle
 * - Alert subscription
 * - Browser notifications
 * - React Query cache invalidation
 *
 * @example
 * ```typescript
 * function DashboardPage() {
 *   useAlertNotifications();
 *
 *   return <div>Dashboard content...</div>;
 * }
 * ```
 */
export function useAlertNotifications(): void {
  const queryClient = useQueryClient();
  const { showNotification, requestPermission, permission } =
    useBrowserNotifications();

  useEffect(() => {
    const token = getToken();

    if (!token) {
      console.warn("No token available for SignalR connection");
      return;
    }

    // Request notification permission
    if (permission === "default") {
      requestPermission();
    }

    // Start SignalR connection
    alertHub
      .start(token)
      .catch((error) => {
        console.error("Failed to start SignalR connection:", error);
      });

    // Subscribe to alert events
    const unsubscribe = alertHub.onAlert((alert: AlertDto) => {
      console.log("Received alert:", alert);

      // Invalidate queries to refresh the alert list
      queryClient.invalidateQueries({
        queryKey: queryKeys.alerts.all(),
      });

      // Show browser notification if permitted
      if (permission === "granted") {
        showNotification("New Alert!", {
          body: `${alert.type} exceeded threshold: ${formatAlertComparison(alert)}`,
          icon: "/alert-icon.svg",
          tag: `alert-${alert.id}`, // Prevent duplicate notifications
          requireInteraction: true, // Keep notification visible until user interacts
        });
      }
    });

    // Cleanup on unmount
    return () => {
      unsubscribe();
      alertHub.stop().catch((error) => {
        console.error("Error stopping SignalR connection:", error);
      });
    };
  }, [queryClient, showNotification, requestPermission, permission]);
}
