"use client";

import LoadingSpinner from "@/components/ui/LoadingSpinner";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import Alert from "@/components/ui/Alert";
import { useAuthRedirect } from "@/lib/hooks/useAuthRedirect";
import { useLoginForm } from "@/lib/hooks/useLoginForm";

export default function LoginPage() {
  const { shouldShowContent } = useAuthRedirect({
    redirectIfAuthenticated: "/dashboard",
  });

  const {
    username,
    password,
    error,
    loading,
    setUsername,
    setPassword,
    handleSubmit,
  } = useLoginForm();

  // Show loading spinner while checking authentication or redirecting
  if (!shouldShowContent) {
    return <LoadingSpinner />;
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
            GreenSpec Alert Service
          </h2>
          <p className="mt-2 text-center text-sm text-gray-600 dark:text-gray-400">
            Sign in to access the dashboard
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <div className="space-y-4">
            <Input
              id="username"
              name="username"
              type="text"
              required
              placeholder="Username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              disabled={loading}
              autoComplete="username"
            />

            <Input
              id="password"
              name="password"
              type="password"
              required
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={loading}
              autoComplete="current-password"
            />
          </div>

          {error && (
            <Alert variant="error">{error}</Alert>
          )}

          <Button
            type="submit"
            disabled={loading}
            loading={loading}
            fullWidth
          >
            Sign in
          </Button>

          <div className="text-center">
            <p className="text-sm text-gray-600 dark:text-gray-400">
              Default credentials:{" "}
              <code className="bg-gray-100 dark:bg-gray-800 px-2 py-1 rounded">
                admin
              </code>{" "}
              /{" "}
              <code className="bg-gray-100 dark:bg-gray-800 px-2 py-1 rounded">
                admin123
              </code>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
}
