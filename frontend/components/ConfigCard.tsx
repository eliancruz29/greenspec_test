"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { configApi } from "@/lib/api";

export default function ConfigCard() {
  const queryClient = useQueryClient();
  const [tempMax, setTempMax] = useState("");
  const [humidityMax, setHumidityMax] = useState("");
  const [isEditing, setIsEditing] = useState(false);

  const { data: config, isLoading } = useQuery({
    queryKey: ["config"],
    queryFn: configApi.getConfig,
  });

  const updateMutation = useMutation({
    mutationFn: configApi.updateConfig,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["config"] });
      setIsEditing(false);
    },
  });

  const handleEdit = () => {
    if (config) {
      setTempMax(config.tempMax.toString());
      setHumidityMax(config.humidityMax.toString());
      setIsEditing(true);
    }
  };

  const handleCancel = () => {
    setIsEditing(false);
    setTempMax("");
    setHumidityMax("");
  };

  const handleSave = () => {
    updateMutation.mutate({
      tempMax: parseFloat(tempMax),
      humidityMax: parseFloat(humidityMax),
    });
  };

  if (isLoading) {
    return (
      <div className="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div className="animate-pulse space-y-4">
          <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-1/4"></div>
          <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-1/2"></div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
          Threshold Configuration
        </h2>
        {!isEditing && (
          <button
            onClick={handleEdit}
            className="px-4 py-2 text-sm font-medium text-indigo-600 dark:text-indigo-400 hover:text-indigo-800 dark:hover:text-indigo-300"
          >
            Edit
          </button>
        )}
      </div>

      {isEditing ? (
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Temperature Max (°C)
            </label>
            <input
              type="number"
              step="0.1"
              value={tempMax}
              onChange={(e) => setTempMax(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 dark:bg-gray-700 dark:text-white"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Humidity Max (%)
            </label>
            <input
              type="number"
              step="0.1"
              value={humidityMax}
              onChange={(e) => setHumidityMax(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 dark:bg-gray-700 dark:text-white"
            />
          </div>
          {updateMutation.isError && (
            <div className="text-sm text-red-600 dark:text-red-400">
              Error updating configuration. Please check your values.
            </div>
          )}
          <div className="flex space-x-2">
            <button
              onClick={handleSave}
              disabled={updateMutation.isPending}
              className="px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
            >
              {updateMutation.isPending ? "Saving..." : "Save"}
            </button>
            <button
              onClick={handleCancel}
              disabled={updateMutation.isPending}
              className="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 rounded-md hover:bg-gray-200 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              Cancel
            </button>
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-2 gap-4">
          <div>
            <p className="text-sm font-medium text-gray-500 dark:text-gray-400">Temperature Max</p>
            <p className="text-2xl font-semibold text-gray-900 dark:text-white">
              {config?.tempMax.toFixed(1)}°C
            </p>
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500 dark:text-gray-400">Humidity Max</p>
            <p className="text-2xl font-semibold text-gray-900 dark:text-white">
              {config?.humidityMax.toFixed(1)}%
            </p>
          </div>
        </div>
      )}

      {!isEditing && config && (
        <div className="mt-4 text-sm text-gray-500 dark:text-gray-400">
          Last updated: {new Date(config.updatedAt).toLocaleString()}
        </div>
      )}
    </div>
  );
}
