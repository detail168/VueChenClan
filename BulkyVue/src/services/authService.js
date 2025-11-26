import axiosInstance from "./axiosInstance";

/**
 * Authentication Service
 * Handles login, logout, token refresh, and user info
 */
export class AuthService {
  /**
   * Login with username and password
   * @param {string} email - User email
   * @param {string} password - User password
   * @returns {Promise} - JWT token and user info
   */
  static async login(email, password) {
    try {
      const response = await axiosInstance.post("/api/auth/login", {
        email,
        password,
      });

      if (response.data.token) {
        localStorage.setItem("authToken", response.data.token);
        localStorage.setItem("refreshToken", response.data.refreshToken || "");
        localStorage.setItem("user", JSON.stringify(response.data.user));
      }

      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || "Login failed");
    }
  }

  /**
   * Logout user
   */
  static logout() {
    localStorage.removeItem("authToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
  }

  /**
   * Get current logged-in user
   * @returns {Object} - User object
   */
  static getCurrentUser() {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  }

  /**
   * Check if user is authenticated
   * @returns {boolean}
   */
  static isAuthenticated() {
    return !!localStorage.getItem("authToken");
  }

  /**
   * Get user roles
   * @returns {string[]} - Array of role names
   */
  static getUserRoles() {
    const user = this.getCurrentUser();
    return user?.roles || [];
  }

  /**
   * Check if user has a specific role
   * @param {string} role - Role name to check
   * @returns {boolean}
   */
  static hasRole(role) {
    return this.getUserRoles().includes(role);
  }

  /**
   * Refresh authentication token
   * @returns {Promise}
   */
  static async refreshToken() {
    try {
      const refreshToken = localStorage.getItem("refreshToken");
      const response = await axiosInstance.post("/api/auth/refresh", {
        refreshToken,
      });

      if (response.data.token) {
        localStorage.setItem("authToken", response.data.token);
      }

      return response.data;
    } catch (error) {
      this.logout();
      throw error;
    }
  }

  /**
   * Get user info from backend
   * @returns {Promise}
   */
  static async getUserInfo() {
    try {
      const response = await axiosInstance.get("/api/auth/user");
      localStorage.setItem("user", JSON.stringify(response.data));
      return response.data;
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to fetch user info"
      );
    }
  }
}

export default AuthService;
