import { useState, useCallback, FormEvent } from "react";
import { useAuth } from "@/lib/auth";
import { isUnauthorizedError, getErrorMessage } from "@/lib/utils/error-guards";

export interface UseLoginFormReturn {
  /**
   * Username field value
   */
  username: string;
  /**
   * Password field value
   */
  password: string;
  /**
   * Error message (if any)
   */
  error: string;
  /**
   * Loading state during submission
   */
  loading: boolean;
  /**
   * Update username
   */
  setUsername: (value: string) => void;
  /**
   * Update password
   */
  setPassword: (value: string) => void;
  /**
   * Handle form submission
   */
  handleSubmit: (e: FormEvent) => Promise<void>;
  /**
   * Clear error message
   */
  clearError: () => void;
}

/**
 * Custom hook for managing login form state and submission
 *
 * Encapsulates all login form logic including:
 * - Form field state
 * - Error handling
 * - Loading state
 * - Submission logic
 *
 * @example
 * ```typescript
 * function LoginPage() {
 *   const {
 *     username,
 *     password,
 *     error,
 *     loading,
 *     setUsername,
 *     setPassword,
 *     handleSubmit
 *   } = useLoginForm();
 *
 *   return (
 *     <form onSubmit={handleSubmit}>
 *       <input value={username} onChange={(e) => setUsername(e.target.value)} />
 *       <input value={password} onChange={(e) => setPassword(e.target.value)} />
 *       <button type="submit" disabled={loading}>Login</button>
 *       {error && <div>{error}</div>}
 *     </form>
 *   );
 * }
 * ```
 */
export function useLoginForm(): UseLoginFormReturn {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  const clearError = useCallback(() => {
    setError("");
  }, []);

  const handleSubmit = useCallback(
    async (e: FormEvent) => {
      e.preventDefault();

      // Clear previous errors
      setError("");

      // Validate inputs
      if (!username.trim()) {
        setError("Username is required");
        return;
      }

      if (!password.trim()) {
        setError("Password is required");
        return;
      }

      setLoading(true);

      try {
        await login({ username, password });
        // Navigation is handled by AuthContext
      } catch (err: unknown) {
        console.error("Login error:", err);

        // Use type guards for proper error handling
        if (isUnauthorizedError(err)) {
          setError("Invalid username or password");
        } else {
          const errorMessage = getErrorMessage(
            err,
            "An error occurred during login. Please try again."
          );
          setError(errorMessage);
        }
      } finally {
        setLoading(false);
      }
    },
    [username, password, login]
  );

  return {
    username,
    password,
    error,
    loading,
    setUsername,
    setPassword,
    handleSubmit,
    clearError,
  };
}
