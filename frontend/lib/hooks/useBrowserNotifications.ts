import { useState, useEffect, useCallback } from "react";

export interface UseBrowserNotificationsReturn {
  /**
   * Current notification permission status
   */
  permission: NotificationPermission;
  /**
   * Whether notifications are supported
   */
  isSupported: boolean;
  /**
   * Request notification permission from user
   */
  requestPermission: () => Promise<NotificationPermission>;
  /**
   * Show a browser notification
   */
  showNotification: (
    title: string,
    options?: NotificationOptions
  ) => Notification | null;
}

/**
 * Custom hook for managing browser notifications
 *
 * Handles permission requests and displaying notifications
 * with proper checks for browser support.
 *
 * @example
 * ```typescript
 * const { permission, showNotification, requestPermission } = useBrowserNotifications();
 *
 * // Request permission
 * await requestPermission();
 *
 * // Show notification
 * showNotification('Alert!', {
 *   body: 'Temperature threshold exceeded',
 *   icon: '/icon.svg'
 * });
 * ```
 */
export function useBrowserNotifications(): UseBrowserNotificationsReturn {
  const isSupported = typeof window !== "undefined" && "Notification" in window;

  const [permission, setPermission] = useState<NotificationPermission>(
    isSupported ? Notification.permission : "denied"
  );

  useEffect(() => {
    if (isSupported) {
      setPermission(Notification.permission);
    }
  }, [isSupported]);

  const requestPermission = useCallback(async (): Promise<NotificationPermission> => {
    if (!isSupported) {
      console.warn("Notifications are not supported in this browser");
      return "denied";
    }

    try {
      const result = await Notification.requestPermission();
      setPermission(result);
      return result;
    } catch (error) {
      console.error("Failed to request notification permission:", error);
      return "denied";
    }
  }, [isSupported]);

  const showNotification = useCallback(
    (title: string, options?: NotificationOptions): Notification | null => {
      if (!isSupported) {
        console.warn("Notifications are not supported");
        return null;
      }

      if (permission !== "granted") {
        console.warn(
          "Notification permission not granted. Current permission:",
          permission
        );
        return null;
      }

      try {
        return new Notification(title, options);
      } catch (error) {
        console.error("Failed to show notification:", error);
        return null;
      }
    },
    [isSupported, permission]
  );

  return {
    permission,
    isSupported,
    requestPermission,
    showNotification,
  };
}
