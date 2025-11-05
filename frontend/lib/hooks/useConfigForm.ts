import { useState, useCallback } from "react";
import { useQuery, useMutation, useQueryClient, UseMutationResult } from "@tanstack/react-query";
import { configApi } from "@/lib/api";
import { queryKeys } from "@/lib/api/query-keys";
import type { ConfigDto, UpdateConfigRequest } from "@/lib/api/types/config.types";

// Re-export types for convenience
export type { ConfigDto, UpdateConfigRequest };

export interface UseConfigFormReturn {
  /**
   * Current config data
   */
  config: ConfigDto | undefined;
  /**
   * Whether config is loading
   */
  isLoading: boolean;
  /**
   * Whether in edit mode
   */
  isEditing: boolean;
  /**
   * Temperature max value (form state)
   */
  tempMax: string;
  /**
   * Humidity max value (form state)
   */
  humidityMax: string;
  /**
   * Update mutation for saving config
   */
  updateMutation: UseMutationResult<ConfigDto, Error, UpdateConfigRequest>;
  /**
   * Enter edit mode and populate form
   */
  handleEdit: () => void;
  /**
   * Cancel edit mode and reset form
   */
  handleCancel: () => void;
  /**
   * Save changes
   */
  handleSave: () => void;
  /**
   * Update temp max field
   */
  setTempMax: (value: string) => void;
  /**
   * Update humidity max field
   */
  setHumidityMax: (value: string) => void;
  /**
   * Whether save is in progress
   */
  isSaving: boolean;
}

/**
 * Custom hook for managing configuration form state
 *
 * Handles:
 * - Config data fetching
 * - Edit mode state
 * - Form field state
 * - Save/cancel actions
 *
 * @example
 * ```typescript
 * function ConfigCard() {
 *   const {
 *     config,
 *     isLoading,
 *     isEditing,
 *     tempMax,
 *     humidityMax,
 *     handleEdit,
 *     handleCancel,
 *     handleSave,
 *     setTempMax,
 *     setHumidityMax
 *   } = useConfigForm();
 *
 *   if (isLoading) return <LoadingSpinner />;
 *
 *   return (
 *     <div>
 *       {isEditing ? (
 *         <form>
 *           <input value={tempMax} onChange={(e) => setTempMax(e.target.value)} />
 *           <button onClick={handleSave}>Save</button>
 *           <button onClick={handleCancel}>Cancel</button>
 *         </form>
 *       ) : (
 *         <div>
 *           <div>Temp: {config?.tempMax}°C</div>
 *           <button onClick={handleEdit}>Edit</button>
 *         </div>
 *       )}
 *     </div>
 *   );
 * }
 * ```
 */
export function useConfigForm(): UseConfigFormReturn {
  const queryClient = useQueryClient();
  const [tempMax, setTempMax] = useState("");
  const [humidityMax, setHumidityMax] = useState("");
  const [isEditing, setIsEditing] = useState(false);

  const { data: config, isLoading } = useQuery({
    queryKey: queryKeys.config.current(),
    queryFn: configApi.getConfig,
  });

  const updateMutation = useMutation<ConfigDto, Error, UpdateConfigRequest>({
    mutationFn: configApi.updateConfig,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.config.all(),
      });
      setIsEditing(false);
    },
  });

  const handleEdit = useCallback(() => {
    if (config) {
      setTempMax(config.tempMax.toString());
      setHumidityMax(config.humidityMax.toString());
      setIsEditing(true);
    }
  }, [config]);

  const handleCancel = useCallback(() => {
    setIsEditing(false);
    setTempMax("");
    setHumidityMax("");
  }, []);

  const handleSave = useCallback(() => {
    const tempValue = parseFloat(tempMax);
    const humidityValue = parseFloat(humidityMax);

    // Basic validation
    if (isNaN(tempValue) || isNaN(humidityValue)) {
      console.error("Invalid numeric values");
      return;
    }

    if (tempValue <= 0 || humidityValue <= 0) {
      console.error("Values must be positive");
      return;
    }

    updateMutation.mutate({
      tempMax: tempValue,
      humidityMax: humidityValue,
    });
  }, [tempMax, humidityMax, updateMutation]);

  return {
    config,
    isLoading,
    isEditing,
    tempMax,
    humidityMax,
    updateMutation,
    handleEdit,
    handleCancel,
    handleSave,
    setTempMax,
    setHumidityMax,
    isSaving: updateMutation.isPending,
  };
}
