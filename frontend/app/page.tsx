"use client";

import LoadingSpinner from "@/components/ui/LoadingSpinner";
import { useAuthRedirect } from "@/lib/hooks/useAuthRedirect";

export default function Home() {
  // Automatically redirect based on authentication status
  useAuthRedirect({
    redirectIfAuthenticated: "/dashboard",
    redirectIfUnauthenticated: "/login",
  });

  // Always show loading while redirecting
  return <LoadingSpinner />;
}
