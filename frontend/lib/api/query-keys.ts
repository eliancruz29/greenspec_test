/**
 * Centralized query key factory for React Query
 *
 * This ensures consistency across the application and makes it easier
 * to invalidate queries when needed.
 */

export const queryKeys = {
  /**
   * All query keys
   */
  all: ["greenspec"] as const,

  /**
   * Alert-related query keys
   */
  alerts: {
    all: () => [...queryKeys.all, "alerts"] as const,
    lists: () => [...queryKeys.alerts.all(), "list"] as const,
    list: (filters?: { status?: string; from?: string; to?: string }) =>
      [...queryKeys.alerts.lists(), filters] as const,
    infinite: (filters?: { status?: string; pageSize?: number }) =>
      [...queryKeys.alerts.all(), "infinite", filters] as const,
    detail: (id: number) => [...queryKeys.alerts.all(), "detail", id] as const,
  },

  /**
   * Configuration-related query keys
   */
  config: {
    all: () => [...queryKeys.all, "config"] as const,
    current: () => [...queryKeys.config.all(), "current"] as const,
    history: () => [...queryKeys.config.all(), "history"] as const,
  },

  /**
   * Authentication-related query keys
   */
  auth: {
    all: () => [...queryKeys.all, "auth"] as const,
    user: () => [...queryKeys.auth.all(), "user"] as const,
    session: () => [...queryKeys.auth.all(), "session"] as const,
  },
} as const;

/**
 * Helper type to extract query key types
 */
export type QueryKeys = typeof queryKeys;

/**
 * Type-safe query key getter
 */
export type AlertQueryKey = ReturnType<typeof queryKeys.alerts.all>;
export type ConfigQueryKey = ReturnType<typeof queryKeys.config.all>;
export type AuthQueryKey = ReturnType<typeof queryKeys.auth.all>;
