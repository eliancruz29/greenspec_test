"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "./use-auth";
import LoadingSpinner from "@/components/ui/LoadingSpinner";

/**
 * Higher-Order Component for protecting routes with authentication
 * Wraps a component and redirects to /login if user is not authenticated
 *
 * @param Component - The component to protect
 * @returns Protected component that requires authentication
 *
 * @example
 * ```tsx
 * function DashboardPage() {
 *   const { username, logout } = useAuth();
 *   return <div>Welcome {username}</div>;
 * }
 *
 * export default withAuth(DashboardPage);
 * ```
 */
export function withAuth<P extends object>(
  Component: React.ComponentType<P>
) {
  return function ProtectedRoute(props: P) {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();

    useEffect(() => {
      if (!loading && !isAuthenticated) {
        router.push("/login");
      }
    }, [isAuthenticated, loading, router]);

    // Show loading spinner while checking authentication
    if (loading || !isAuthenticated) {
      return <LoadingSpinner />;
    }

    // Render the protected component
    return <Component {...props} />;
  };
}
