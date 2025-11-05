"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { alertsApi, type AlertDto } from "@/lib/api";
import Card, { CardContent, CardHeader } from "@/components/ui/Card";
import Button from "@/components/ui/Button";
import { useInfiniteAlerts } from "@/lib/hooks/useInfiniteAlerts";
import {
  getStatusBadgeClasses,
  getTypeBadgeClasses,
  formatAlertValue,
} from "@/lib/utils/alert-formatters";
import { formatDateTime } from "@/lib/utils/date-formatters";
import { queryKeys } from "@/lib/api/query-keys";

export default function AlertsTable() {
  const queryClient = useQueryClient();

  const {
    alerts,
    totalCount,
    statusFilter,
    setStatusFilter,
    observerTarget,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
  } = useInfiniteAlerts({ pageSize: 10 });

  const acknowledgeMutation = useMutation({
    mutationFn: (id: number) => alertsApi.acknowledgeAlert(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.alerts.all() });
    },
  });

  const handleFilterChange = (filter: string) => {
    setStatusFilter(filter);
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent>
          <div className="animate-pulse space-y-4">
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-1/4"></div>
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded"></div>
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded"></div>
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded"></div>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex justify-between items-center">
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
            Alerts ({totalCount})
          </h2>
          <div className="flex space-x-2">
            <Button
              onClick={() => handleFilterChange("")}
              variant={statusFilter === "" ? "primary" : "secondary"}
              size="sm"
            >
              All
            </Button>
            <Button
              onClick={() => handleFilterChange("open")}
              variant={statusFilter === "open" ? "primary" : "secondary"}
              size="sm"
            >
              Open
            </Button>
            <Button
              onClick={() => handleFilterChange("acknowledged")}
              variant={statusFilter === "acknowledged" ? "primary" : "secondary"}
              size="sm"
            >
              Acknowledged
            </Button>
          </div>
        </div>
      </CardHeader>

      <div className="overflow-x-auto">
        {alerts && alerts.length > 0 ? (
          <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead className="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Type
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Value
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Threshold
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Created At
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Action
                </th>
              </tr>
            </thead>
            <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              {alerts.map((alert: AlertDto) => (
                <tr key={alert.id} className="hover:bg-gray-50 dark:hover:bg-gray-700">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`inline-flex px-2 text-xs leading-5 font-semibold rounded-full ${getTypeBadgeClasses(alert.type)}`}>
                      {alert.type}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
                    {formatAlertValue(alert)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
                    {formatAlertValue({ ...alert, value: alert.threshold })}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
                    {formatDateTime(alert.createdAt)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeClasses(
                        alert.status
                      )}`}
                    >
                      {alert.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    {alert.status.toLowerCase() === "open" && (
                      <Button
                        onClick={() => acknowledgeMutation.mutate(alert.id)}
                        disabled={acknowledgeMutation.isPending}
                        variant="ghost"
                        size="sm"
                      >
                        Acknowledge
                      </Button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <div className="px-6 py-12 text-center">
            <p className="text-gray-500 dark:text-gray-400">
              {statusFilter ? `No ${statusFilter} alerts found` : "No alerts found"}
            </p>
          </div>
        )}

        {/* Infinite scroll trigger */}
        {alerts && alerts.length > 0 && (
          <div ref={observerTarget} className="px-6 py-4 text-center">
            {isFetchingNextPage && (
              <div className="flex items-center justify-center space-x-2">
                <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-indigo-600"></div>
                <span className="text-sm text-gray-500 dark:text-gray-400">Loading more...</span>
              </div>
            )}
            {!hasNextPage && alerts.length > 0 && (
              <p className="text-sm text-gray-500 dark:text-gray-400">
                Showing all {alerts.length} of {totalCount} alerts
              </p>
            )}
          </div>
        )}
      </div>
    </Card>
  );
}
