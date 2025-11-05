"use client";

import { useAuth, withAuth } from "@/lib/auth";
import Button from "@/components/ui/Button";
import ConfigCard from "@/components/ConfigCard";
import AlertsTable from "@/components/AlertsTable";
import { useAlertNotifications } from "@/lib/hooks/useAlertNotifications";

function DashboardPage() {
  const { username, logout } = useAuth();

  // Set up real-time alert notifications
  useAlertNotifications();

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
              <Button onClick={logout} size="sm">
                Logout
              </Button>
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
