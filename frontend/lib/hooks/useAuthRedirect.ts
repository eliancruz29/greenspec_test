import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";

export interface UseAuthRedirectOptions {
  /**
   * Where to redirect if user is authenticated
   */
  redirectIfAuthenticated?: string;
  /**
   * Where to redirect if user is not authenticated
   */
  redirectIfUnauthenticated?: string;
}

export interface UseAuthRedirectReturn {
  /**
   * Whether to show the page content
   * (false while checking auth or redirecting)
   */
  shouldShowContent: boolean;
}

/**
 * Custom hook for handling authentication-based redirects
 *
 * Simplifies the common pattern of redirecting users based on
 * their authentication status.
 *
 * @param options - Redirect configuration
 *
 * @example
 * ```typescript
 * // Redirect authenticated users away from login page
 * function LoginPage() {
 *   const { shouldShowContent } = useAuthRedirect({
 *     redirectIfAuthenticated: '/dashboard'
 *   });
 *
 *   if (!shouldShowContent) return <LoadingSpinner />;
 *
 *   return <LoginForm />;
 * }
 * ```
 *
 * @example
 * ```typescript
 * // Redirect unauthenticated users to login
 * function ProtectedPage() {
 *   const { shouldShowContent } = useAuthRedirect({
 *     redirectIfUnauthenticated: '/login'
 *   });
 *
 *   if (!shouldShowContent) return <LoadingSpinner />;
 *
 *   return <ProtectedContent />;
 * }
 * ```
 */
export function useAuthRedirect(
  options: UseAuthRedirectOptions = {}
): UseAuthRedirectReturn {
  const { redirectIfAuthenticated, redirectIfUnauthenticated } = options;
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();
  const [shouldShowContent, setShouldShowContent] = useState(false);

  useEffect(() => {
    // Wait for auth state to load
    if (loading) {
      setShouldShowContent(false);
      return;
    }

    // Handle authenticated user
    if (isAuthenticated && redirectIfAuthenticated) {
      router.push(redirectIfAuthenticated);
      setShouldShowContent(false);
      return;
    }

    // Handle unauthenticated user
    if (!isAuthenticated && redirectIfUnauthenticated) {
      router.push(redirectIfUnauthenticated);
      setShouldShowContent(false);
      return;
    }

    // No redirect needed, show content
    setShouldShowContent(true);
  }, [
    isAuthenticated,
    loading,
    redirectIfAuthenticated,
    redirectIfUnauthenticated,
    router,
  ]);

  return {
    shouldShowContent,
  };
}
