/**
 * Storage abstraction for authentication data
 *
 * Centralizes all localStorage operations for auth,
 * making it easier to:
 * - Test (can mock this module)
 * - Change storage mechanism (e.g., sessionStorage, cookies)
 * - Add encryption/security
 */

const STORAGE_KEYS = {
  TOKEN: "token",
  USER: "user",
  REFRESH_TOKEN: "refreshToken",
} as const;

/**
 * Store authentication token
 */
export function setToken(token: string): void {
  try {
    localStorage.setItem(STORAGE_KEYS.TOKEN, token);
  } catch (error) {
    console.error("Failed to store token:", error);
  }
}

/**
 * Get authentication token
 */
export function getToken(): string | null {
  try {
    return localStorage.getItem(STORAGE_KEYS.TOKEN);
  } catch (error) {
    console.error("Failed to retrieve token:", error);
    return null;
  }
}

/**
 * Remove authentication token
 */
export function removeToken(): void {
  try {
    localStorage.removeItem(STORAGE_KEYS.TOKEN);
  } catch (error) {
    console.error("Failed to remove token:", error);
  }
}

/**
 * Check if user is authenticated (has token)
 */
export function hasToken(): boolean {
  return getToken() !== null;
}

/**
 * Store user data
 */
export function setUser(user: Record<string, unknown>): void {
  try {
    localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
  } catch (error) {
    console.error("Failed to store user data:", error);
  }
}

/**
 * Get user data
 */
export function getUser<T = Record<string, unknown>>(): T | null {
  try {
    const userData = localStorage.getItem(STORAGE_KEYS.USER);
    return userData ? JSON.parse(userData) : null;
  } catch (error) {
    console.error("Failed to retrieve user data:", error);
    return null;
  }
}

/**
 * Remove user data
 */
export function removeUser(): void {
  try {
    localStorage.removeItem(STORAGE_KEYS.USER);
  } catch (error) {
    console.error("Failed to remove user data:", error);
  }
}

/**
 * Store refresh token
 */
export function setRefreshToken(token: string): void {
  try {
    localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, token);
  } catch (error) {
    console.error("Failed to store refresh token:", error);
  }
}

/**
 * Get refresh token
 */
export function getRefreshToken(): string | null {
  try {
    return localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
  } catch (error) {
    console.error("Failed to retrieve refresh token:", error);
    return null;
  }
}

/**
 * Remove refresh token
 */
export function removeRefreshToken(): void {
  try {
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
  } catch (error) {
    console.error("Failed to remove refresh token:", error);
  }
}

/**
 * Clear all authentication data
 */
export function clearAuth(): void {
  removeToken();
  removeUser();
  removeRefreshToken();
}

/**
 * Initialize auth from storage
 */
export function initializeAuth(): {
  token: string | null;
  user: Record<string, unknown> | null;
} {
  return {
    token: getToken(),
    user: getUser(),
  };
}

/**
 * Check if storage is available
 */
export function isStorageAvailable(): boolean {
  try {
    const test = "__storage_test__";
    localStorage.setItem(test, test);
    localStorage.removeItem(test);
    return true;
  } catch {
    return false;
  }
}
