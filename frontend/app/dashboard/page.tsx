"use client";

import { useEffect } from "react";
import { useAuth, withAuth } from "@/lib/auth";
import ConfigCard from "@/components/ConfigCard";
import AlertsTable from "@/components/AlertsTable";
import { alertHub } from "@/lib/signalr";
import { useQueryClient } from "@tanstack/react-query";
import type { AlertDto } from "@/lib/api";

function DashboardPage() {
  const { username, logout } = useAuth();
  const queryClient = useQueryClient();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) return;

    // Subscribe to alert notifications
    const unsubscribe = alertHub.onAlert((alert: AlertDto) => {
      // Invalidate alerts query to refetch
      queryClient.invalidateQueries({ queryKey: ["alerts"] });

      // Show notification
      if ("Notification" in window && Notification.permission === "granted") {
        new Notification("New Alert!", {
          body: `${alert.type} exceeded threshold: ${alert.value.toFixed(1)} > ${alert.threshold.toFixed(1)}`,
          icon: "/alert-icon.svg",
        });
      }
    });

    // Start the SignalR connection
    alertHub.start(token);

    // Request notification permission
    if ("Notification" in window && Notification.permission === "default") {
      Notification.requestPermission();
    }

    return () => {
      // Unsubscribe from alerts and stop connection
      unsubscribe();
      alertHub.stop();
    };
  }, [queryClient]);

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
      {/* Navigation */}
      <nav className="bg-white dark:bg-gray-800 shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                GreenSpec Dashboard
              </h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-gray-700 dark:text-gray-300">
                Welcome, {username}
              </span>
              <button
                onClick={logout}
                className="px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="space-y-6">
            {/* Configuration Card */}
            <ConfigCard />

            {/* Alerts Table */}
            <AlertsTable />
          </div>
        </div>
      </main>
    </div>
  );
}

export default withAuth(DashboardPage);
