"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import ConfigCard from "@/components/ConfigCard";
import AlertsTable from "@/components/AlertsTable";
import { alertHub } from "@/lib/signalr";
import { useQueryClient } from "@tanstack/react-query";
import type { AlertDto } from "@/lib/api";

export default function DashboardPage() {
  const { isAuthenticated, username, logout, loading } = useAuth();
  const router = useRouter();
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, loading, router]);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token && isAuthenticated) {
      alertHub.start(token, (alert: AlertDto) => {
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

      // Request notification permission
      if ("Notification" in window && Notification.permission === "default") {
        Notification.requestPermission();
      }

      return () => {
        alertHub.stop();
      };
    }
  }, [isAuthenticated, queryClient]);

  if (loading || !isAuthenticated) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto"></div>
          <p className="mt-4 text-gray-600 dark:text-gray-400">Loading...</p>
        </div>
      </div>
    );
  }

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
