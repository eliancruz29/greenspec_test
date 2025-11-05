"use client";

import Card, { CardContent, CardHeader } from "@/components/ui/Card";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import Alert from "@/components/ui/Alert";
import { useConfigForm } from "@/lib/hooks/useConfigForm";
import { formatDateTime } from "@/lib/utils/date-formatters";

export default function ConfigCard() {
  const {
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
    isSaving,
  } = useConfigForm();

  if (isLoading) {
    return (
      <Card>
        <CardContent>
          <div className="animate-pulse space-y-4">
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-1/4"></div>
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-1/2"></div>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex justify-between items-center">
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
            Threshold Configuration
          </h2>
          {!isEditing && (
            <Button onClick={handleEdit} variant="ghost" size="sm">
              Edit
            </Button>
          )}
        </div>
      </CardHeader>

      <CardContent>
        {isEditing ? (
          <div className="space-y-4">
            <Input
              label="Temperature Max (°C)"
              type="number"
              step="0.1"
              value={tempMax}
              onChange={(e) => setTempMax(e.target.value)}
              disabled={isSaving}
            />

            <Input
              label="Humidity Max (%)"
              type="number"
              step="0.1"
              value={humidityMax}
              onChange={(e) => setHumidityMax(e.target.value)}
              disabled={isSaving}
            />

            {updateMutation.isError && (
              <Alert variant="error">
                Error updating configuration. Please check your values.
              </Alert>
            )}

            <div className="flex space-x-2">
              <Button
                onClick={handleSave}
                disabled={isSaving}
                loading={isSaving}
              >
                Save
              </Button>
              <Button
                onClick={handleCancel}
                disabled={isSaving}
                variant="secondary"
              >
                Cancel
              </Button>
            </div>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <p className="text-sm font-medium text-gray-500 dark:text-gray-400">
                  Temperature Max
                </p>
                <p className="text-2xl font-semibold text-gray-900 dark:text-white">
                  {config?.tempMax.toFixed(1)}°C
                </p>
              </div>
              <div>
                <p className="text-sm font-medium text-gray-500 dark:text-gray-400">
                  Humidity Max
                </p>
                <p className="text-2xl font-semibold text-gray-900 dark:text-white">
                  {config?.humidityMax.toFixed(1)}%
                </p>
              </div>
            </div>

            {config?.updatedAt && (
              <div className="mt-4 text-sm text-gray-500 dark:text-gray-400">
                Last updated: {formatDateTime(config.updatedAt)}
              </div>
            )}
          </>
        )}
      </CardContent>
    </Card>
  );
}
