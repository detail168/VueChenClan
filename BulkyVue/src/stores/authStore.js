import { defineStore } from "pinia";
import { ref, computed } from "vue";
import AuthService from "@/services/authService";

export const useAuthStore = defineStore("auth", () => {
  const user = ref(AuthService.getCurrentUser());
  const token = ref(localStorage.getItem("authToken"));
  const isLoading = ref(false);
  const error = ref(null);

  const isAuthenticated = computed(() => !!token.value);
  const userRoles = computed(() => user.value?.roles || []);
  const isAdmin = computed(() => userRoles.value.includes("Admin"));
  const isCustomer = computed(() => userRoles.value.includes("Customer"));

  const login = async (email, password) => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await AuthService.login(email, password);
      user.value = response.user;
      token.value = response.token;
      return response;
    } catch (err) {
      error.value = err.message;
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  const logout = () => {
    AuthService.logout();
    user.value = null;
    token.value = null;
    error.value = null;
  };

  const refreshUser = async () => {
    try {
      const userInfo = await AuthService.getUserInfo();
      user.value = userInfo;
      return userInfo;
    } catch (err) {
      error.value = err.message;
      throw err;
    }
  };

  const hasRole = (role) => {
    return userRoles.value.includes(role);
  };

  const clearError = () => {
    error.value = null;
  };

  return {
    user,
    token,
    isLoading,
    error,
    isAuthenticated,
    userRoles,
    isAdmin,
    isCustomer,
    login,
    logout,
    refreshUser,
    hasRole,
    clearError,
  };
});
