import React from "react";

export interface LoadingSpinnerProps {
  /**
   * Whether to render fullscreen (centered on entire viewport)
   */
  fullScreen?: boolean;
  /**
   * Custom message to display below spinner
   */
  message?: string;
  /**
   * Size variant of the spinner
   */
  size?: "sm" | "md" | "lg";
  /**
   * Optional className for customization
   */
  className?: string;
}

const sizeClasses = {
  sm: "h-6 w-6 border-2",
  md: "h-12 w-12 border-b-2",
  lg: "h-16 w-16 border-b-4",
};

export const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
  fullScreen = true,
  message = "Loading...",
  size = "md",
  className = "",
}) => {
  const spinnerContent = (
    <div className={`text-center ${className}`}>
      <div
        className={`animate-spin rounded-full ${sizeClasses[size]} border-indigo-600 mx-auto`}
        role="status"
        aria-label="Loading"
      />
      {message && (
        <p className="mt-4 text-gray-600 dark:text-gray-400">{message}</p>
      )}
    </div>
  );

  if (fullScreen) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        {spinnerContent}
      </div>
    );
  }

  return spinnerContent;
};

export default LoadingSpinner;
