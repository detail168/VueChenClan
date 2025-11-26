import { defineStore } from "pinia";
import { ref, computed } from "vue";
import { ConfigService, KindnessService } from "@/services/apiService";

export const useKindnessStore = defineStore("kindness", () => {
  const positions = ref([]);
  const config = ref(null);
  const isLoading = ref(false);
  const error = ref(null);
  const selectedPosition = ref(null);
  const filters = ref({
    floor: null,
    section: null,
    status: null,
  });

  const totalCount = computed(() => positions.value.length);
  const availablePositions = computed(() =>
    positions.value.filter((p) => p.status === "Available")
  );
  const filteredPositions = computed(() => {
    let result = positions.value;

    if (filters.value.floor) {
      result = result.filter((p) => p.floor === filters.value.floor);
    }
    if (filters.value.section) {
      result = result.filter((p) => p.section === filters.value.section);
    }
    if (filters.value.status) {
      result = result.filter((p) => p.status === filters.value.status);
    }

    return result;
  });

  const fetchConfig = async () => {
    try {
      config.value = await ConfigService.getKindnessConfig();
    } catch (err) {
      error.value = "Failed to load configuration";
      console.error(err);
    }
  };

  const fetchAll = async () => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await KindnessService.getAll();
      // Handle both array and object with data property
      positions.value = Array.isArray(response) ? response : (response.data || []);
    } catch (err) {
      error.value = err.message;
      positions.value = []; // Ensure positions is always an array
    } finally {
      isLoading.value = false;
    }
  };

  const getById = async (id) => {
    try {
      selectedPosition.value = await KindnessService.getById(id);
      return selectedPosition.value;
    } catch (err) {
      error.value = err.message;
      throw err;
    }
  };

  const create = async (position) => {
    isLoading.value = true;
    error.value = null;

    try {
      const newPosition = await KindnessService.create(position);
      positions.value.push(newPosition);
      return newPosition;
    } catch (err) {
      error.value = err.message;
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  const update = async (id, position) => {
    isLoading.value = true;
    error.value = null;

    try {
      const updatedPosition = await KindnessService.update(id, position);
      const index = positions.value.findIndex((p) => p.id === id);
      if (index !== -1) {
        positions.value[index] = updatedPosition;
      }
      return updatedPosition;
    } catch (err) {
      error.value = err.message;
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  const deletePosition = async (id) => {
    isLoading.value = true;
    error.value = null;

    try {
      await KindnessService.delete(id);
      positions.value = positions.value.filter((p) => p.id !== id);
    } catch (err) {
      error.value = err.message;
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  const importExcel = async (file) => {
    isLoading.value = true;
    error.value = null;

    try {
      const result = await KindnessService.importExcel(file);
      await fetchAll();
      return result;
    } catch (err) {
      error.value = err.message;
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  const exportExcel = async () => {
    try {
      const blob = await KindnessService.exportExcel();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `kindness_positions_${
        new Date().toISOString().split("T")[0]
      }.xlsx`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      a.remove();
    } catch (err) {
      error.value = err.message;
      throw err;
    }
  };

  const setFilter = (key, value) => {
    filters.value[key] = value;
  };

  const clearFilters = () => {
    filters.value = {
      floor: null,
      section: null,
      status: null,
    };
  };

  const clearError = () => {
    error.value = null;
  };

  return {
    positions,
    config,
    isLoading,
    error,
    selectedPosition,
    filters,
    totalCount,
    availablePositions,
    filteredPositions,
    fetchConfig,
    fetchAll,
    getById,
    create,
    update,
    deletePosition,
    importExcel,
    exportExcel,
    setFilter,
    clearFilters,
    clearError,
  };
});
