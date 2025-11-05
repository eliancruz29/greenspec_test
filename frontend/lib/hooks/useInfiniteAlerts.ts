import { useState, useRef, useCallback, useEffect } from "react";
import { useInfiniteQuery } from "@tanstack/react-query";
import { alertsApi, type AlertDto } from "@/lib/api";
import { queryKeys } from "@/lib/api/query-keys";
import type { PagedResultDto } from "@/lib/api/types/common.types";

// Type alias for better readability
export type PagedResponse = PagedResultDto<AlertDto>;

export interface UseInfiniteAlertsReturn {
  /**
   * Flattened array of all loaded alerts
   */
  alerts: AlertDto[];
  /**
   * Total count of alerts matching the filter
   */
  totalCount: number;
  /**
   * Current status filter
   */
  statusFilter: string;
  /**
   * Update status filter
   */
  setStatusFilter: (filter: string) => void;
  /**
   * Ref for intersection observer target
   */
  observerTarget: React.RefObject<HTMLDivElement | null>;
  /**
   * Whether there are more pages to load
   */
  hasNextPage: boolean | undefined;
  /**
   * Whether currently fetching next page
   */
  isFetchingNextPage: boolean;
  /**
   * Whether initial load is in progress
   */
  isLoading: boolean;
}

export interface UseInfiniteAlertsOptions {
  /**
   * Number of items per page
   */
  pageSize?: number;
  /**
   * Refetch interval in milliseconds
   */
  refetchInterval?: number;
  /**
   * Initial status filter
   */
  initialStatusFilter?: string;
}

/**
 * Custom hook for managing infinite scrolling alerts list
 *
 * Handles:
 * - Infinite query setup
 * - Filter state management
 * - Intersection observer for auto-loading
 * - Data flattening
 *
 * @param options - Configuration options
 *
 * @example
 * ```typescript
 * function AlertsTable() {
 *   const {
 *     alerts,
 *     totalCount,
 *     statusFilter,
 *     setStatusFilter,
 *     observerTarget,
 *     hasNextPage,
 *     isLoading
 *   } = useInfiniteAlerts({ pageSize: 10 });
 *
 *   return (
 *     <div>
 *       <FilterButtons onChange={setStatusFilter} />
 *       {alerts.map(alert => <AlertRow key={alert.id} alert={alert} />)}
 *       <div ref={observerTarget} />
 *     </div>
 *   );
 * }
 * ```
 */
export function useInfiniteAlerts(
  options: UseInfiniteAlertsOptions = {}
): UseInfiniteAlertsReturn {
  const {
    pageSize = 10,
    refetchInterval = 30000,
    initialStatusFilter = "",
  } = options;

  const [statusFilter, setStatusFilter] = useState<string>(initialStatusFilter);
  const observerTarget = useRef<HTMLDivElement>(null);

  const queryResult = useInfiniteQuery({
    queryKey: queryKeys.alerts.infinite({ status: statusFilter || undefined, pageSize }),
    queryFn: ({ pageParam = 1 }) =>
      alertsApi.getPagedAlerts({
        status: statusFilter || undefined,
        pageNumber: pageParam,
        pageSize: pageSize,
      }),
    getNextPageParam: (lastPage) => {
      return lastPage.hasNextPage ? lastPage.pageNumber + 1 : undefined;
    },
    initialPageParam: 1,
    refetchInterval,
  });

  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
  } = queryResult;

  // Flatten all pages into a single array of alerts
  const alerts = data?.pages.flatMap((page) => page.data) ?? [];
  const totalCount = data?.pages[0]?.totalCount ?? 0;

  // Intersection Observer callback for infinite scroll
  const handleObserver = useCallback(
    (entries: IntersectionObserverEntry[]) => {
      const [target] = entries;
      if (target.isIntersecting && hasNextPage && !isFetchingNextPage) {
        fetchNextPage();
      }
    },
    [fetchNextPage, hasNextPage, isFetchingNextPage]
  );

  // Setup intersection observer
  useEffect(() => {
    const element = observerTarget.current;
    if (!element) return;

    const observer = new IntersectionObserver(handleObserver, {
      threshold: 0.1,
    });

    observer.observe(element);
    return () => observer.disconnect();
  }, [handleObserver]);

  return {
    alerts,
    totalCount,
    statusFilter,
    setStatusFilter,
    observerTarget,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
  };
}
