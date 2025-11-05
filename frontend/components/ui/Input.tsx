import React from "react";

export interface InputProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "size"> {
  /**
   * Label text for the input
   */
  label?: string;
  /**
   * Error message to display below input
   */
  error?: string;
  /**
   * Size variant
   */
  size?: "sm" | "md" | "lg";
  /**
   * Full width input
   */
  fullWidth?: boolean;
}

const sizeClasses = {
  sm: "px-3 py-1.5 text-sm",
  md: "px-4 py-2 text-base",
  lg: "px-5 py-3 text-lg",
};

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  (
    {
      label,
      error,
      size = "md",
      fullWidth = true,
      className = "",
      id,
      ...props
    },
    ref
  ) => {
    const inputId = id || label?.toLowerCase().replace(/\s+/g, "-");

    const baseClasses = `
      ${sizeClasses[size]}
      ${fullWidth ? "w-full" : ""}
      border rounded-md shadow-sm
      bg-white dark:bg-gray-800
      text-gray-900 dark:text-white
      placeholder-gray-400 dark:placeholder-gray-500
      focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent
      disabled:opacity-50 disabled:cursor-not-allowed
      transition-colors
      ${error
        ? "border-red-500 dark:border-red-500"
        : "border-gray-300 dark:border-gray-700"
      }
      ${className}
    `.trim().replace(/\s+/g, " ");

    return (
      <div className={fullWidth ? "w-full" : ""}>
        {label && (
          <label
            htmlFor={inputId}
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={inputId}
          className={baseClasses}
          {...props}
        />
        {error && (
          <p className="mt-1 text-sm text-red-600 dark:text-red-400">
            {error}
          </p>
        )}
      </div>
    );
  }
);

Input.displayName = "Input";

export default Input;
