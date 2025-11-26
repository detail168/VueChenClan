import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/authStore";

// Layout Components
import AppLayout from "@/components/Layout/AppLayout.vue";

// View Components
import LoginView from "@/views/LoginView.vue";
import DashboardView from "@/views/DashboardView.vue";

// Ancestral Views
import AncestralListView from "@/views/AncestralListView.vue";
import AncestralFormView from "@/views/AncestralFormView.vue";
import AncestralGridView from "@/views/AncestralGridView.vue";

// Kindness Views
import KindnessListView from "@/views/KindnessListView.vue";
import KindnessFormView from "@/views/KindnessFormView.vue";
import KindnessGridView from "@/views/KindnessGridView.vue";

// Error Views
import NotFoundView from "@/views/NotFoundView.vue";
import UnauthorizedView from "@/views/UnauthorizedView.vue";

const routes = [
  {
    path: "/login",
    name: "Login",
    component: LoginView,
    meta: { requiresAuth: false },
  },

  {
    path: "/",
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: "",
        name: "Dashboard",
        component: DashboardView,
        meta: { requiresAuth: true },
      },

      // Ancestral Routes
      {
        path: "ancestral",
        name: "AncestralList",
        component: AncestralListView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
      {
        path: "ancestral/new",
        name: "AncestralCreate",
        component: AncestralFormView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
      {
        path: "ancestral/:id/edit",
        name: "AncestralEdit",
        component: AncestralFormView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
      {
        path: "ancestral/grid",
        name: "AncestralGrid",
        component: AncestralGridView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },

      // Kindness Routes
      {
        path: "kindness",
        name: "KindnessList",
        component: KindnessListView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
      {
        path: "kindness/new",
        name: "KindnessCreate",
        component: KindnessFormView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
      {
        path: "kindness/:id/edit",
        name: "KindnessEdit",
        component: KindnessFormView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
      {
        path: "kindness/grid",
        name: "KindnessGrid",
        component: KindnessGridView,
        meta: { requiresAuth: true, requiresRole: "Admin" },
      },
    ],
  },

  {
    path: "/unauthorized",
    name: "Unauthorized",
    component: UnauthorizedView,
  },

  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFoundView,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

/**
 * Navigation Guard: Check authentication and authorization
 */
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();

  // Check if route requires authentication
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: "Login", query: { redirect: to.fullPath } });
    return;
  }

  // Check if route requires specific role
  if (to.meta.requiresRole && !authStore.hasRole(to.meta.requiresRole)) {
    next({ name: "Unauthorized" });
    return;
  }

  // Allow access
  next();
});

export default router;
