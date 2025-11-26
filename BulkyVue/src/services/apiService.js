import axiosInstance from "./axiosInstance";

/**
 * Configuration Service
 * Fetches layout configurations for Ancestral and Kindness modules
 */
export class ConfigService {
  static async getAncestralConfig() {
    try {
      const response = await axiosInstance.get("/api/config/ancestral");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch Ancestral config:", error);
      return null;
    }
  }

  static async getKindnessConfig() {
    try {
      const response = await axiosInstance.get("/api/config/kindness");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch Kindness config:", error);
      return null;
    }
  }

  static async getAppSettings() {
    try {
      const response = await axiosInstance.get("/api/config/app-settings");
      return response.data;
    } catch (error) {
      console.error("Failed to fetch app settings:", error);
      return null;
    }
  }
}

/**
 * Ancestral Position Service
 * CRUD operations for ancestral positions
 */
export class AncestralService {
  static async getAll(params = {}) {
    try {
      const response = await axiosInstance.get("/api/admin/ancestral", {
        params,
      });
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to fetch positions"
      );
    }
  }

  static async getById(id) {
    try {
      const response = await axiosInstance.get(`/api/admin/ancestral/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to fetch position"
      );
    }
  }

  static async create(position) {
    try {
      const response = await axiosInstance.post(
        "/api/admin/ancestral",
        position
      );
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to create position"
      );
    }
  }

  static async update(id, position) {
    try {
      const response = await axiosInstance.put(
        `/api/admin/ancestral/${id}`,
        position
      );
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to update position"
      );
    }
  }

  static async delete(id) {
    try {
      const response = await axiosInstance.delete(`/api/admin/ancestral/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to delete position"
      );
    }
  }

  static async importExcel(file) {
    try {
      const formData = new FormData();
      formData.append("file", file);
      const response = await axiosInstance.post(
        "/api/admin/ancestral/import",
        formData,
        {
          headers: { "Content-Type": "multipart/form-data" },
        }
      );
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to import Excel"
      );
    }
  }

  static async exportExcel() {
    try {
      const response = await axiosInstance.get("/api/admin/ancestral/export", {
        responseType: "blob",
      });
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to export Excel"
      );
    }
  }
}

/**
 * Kindness Position Service
 * CRUD operations for kindness positions
 */
export class KindnessService {
  static async getAll(params = {}) {
    try {
      const response = await axiosInstance.get("/api/admin/kindness", {
        params,
      });
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to fetch positions"
      );
    }
  }

  static async getById(id) {
    try {
      const response = await axiosInstance.get(`/api/admin/kindness/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to fetch position"
      );
    }
  }

  static async create(position) {
    try {
      const response = await axiosInstance.post(
        "/api/admin/kindness",
        position
      );
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to create position"
      );
    }
  }

  static async update(id, position) {
    try {
      const response = await axiosInstance.put(
        `/api/admin/kindness/${id}`,
        position
      );
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to update position"
      );
    }
  }

  static async delete(id) {
    try {
      const response = await axiosInstance.delete(`/api/admin/kindness/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to delete position"
      );
    }
  }

  static async importExcel(file) {
    try {
      const formData = new FormData();
      formData.append("file", file);
      const response = await axiosInstance.post(
        "/api/admin/kindness/import",
        formData,
        {
          headers: { "Content-Type": "multipart/form-data" },
        }
      );
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to import Excel"
      );
    }
  }

  static async exportExcel() {
    try {
      const response = await axiosInstance.get("/api/admin/kindness/export", {
        responseType: "blob",
      });
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to export Excel"
      );
    }
  }
}

export default {
  ConfigService,
  AncestralService,
  KindnessService,
};
