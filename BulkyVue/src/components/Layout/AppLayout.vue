<template>
  <div class="d-flex flex-column min-vh-100">
    <!-- Navbar -->
    <nav class="navbar navbar-expand-md navbar-dark bg-primary sticky-top">
      <div class="container-fluid">
        <a class="navbar-brand" href="/">
          <img src="/images/ChenClan0.jpg" alt="Logo" style="width: 30px; height: auto;" class="me-2" />
          <strong>BulkyBook</strong>
        </a>
        
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <!-- Admin Links -->
            <li class="nav-item" v-if="authStore.isAdmin">
              <a class="nav-link" href="#" @click.prevent="toggleAdminMenu" aria-current="page">
                管理員工具 <i class="bi bi-chevron-down ms-1"></i>
              </a>
            </li>
            
            <!-- User Profile -->
            <li class="nav-item dropdown">
              <a class="nav-link dropdown-toggle" href="#" id="userDropdown" role="button" data-bs-toggle="dropdown">
                <i class="bi bi-person-circle me-1"></i>
                {{ authStore.user?.email || 'User' }}
              </a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li><a class="dropdown-item" href="#" @click.prevent="logout">登出</a></li>
              </ul>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Sidebar + Main Content -->
    <div class="d-flex flex-grow-1">
      <!-- Sidebar -->
      <nav class="bg-light border-end" style="width: 250px; min-height: calc(100vh - 56px);">
        <div class="list-group list-group-flush">
          <router-link to="/" class="list-group-item list-group-item-action">
            <i class="bi bi-house-door me-2"></i>儀表板
          </router-link>
          
          <!-- Ancestral Section -->
          <div class="list-group-item" v-if="authStore.isAdmin">
            <strong class="d-block mb-2">祖先牌位</strong>
            <ul class="list-unstyled ms-3">
              <li class="mb-1">
                <router-link to="/ancestral" class="text-decoration-none">
                  <i class="bi bi-list me-2"></i>清單
                </router-link>
              </li>
              <li class="mb-1">
                <router-link to="/ancestral/new" class="text-decoration-none">
                  <i class="bi bi-plus-circle me-2"></i>新增
                </router-link>
              </li>
              <li class="mb-1">
                <router-link to="/ancestral/grid" class="text-decoration-none">
                  <i class="bi bi-grid-3x3-gap me-2"></i>圖形檢視
                </router-link>
              </li>
            </ul>
          </div>

          <!-- Kindness Section -->
          <div class="list-group-item" v-if="authStore.isAdmin">
            <strong class="d-block mb-2">懷恩塔</strong>
            <ul class="list-unstyled ms-3">
              <li class="mb-1">
                <router-link to="/kindness" class="text-decoration-none">
                  <i class="bi bi-list me-2"></i>清單
                </router-link>
              </li>
              <li class="mb-1">
                <router-link to="/kindness/new" class="text-decoration-none">
                  <i class="bi bi-plus-circle me-2"></i>新增
                </router-link>
              </li>
              <li class="mb-1">
                <router-link to="/kindness/grid" class="text-decoration-none">
                  <i class="bi bi-grid-3x3-gap me-2"></i>圖形檢視
                </router-link>
              </li>
            </ul>
          </div>
        </div>
      </nav>

      <!-- Main Content -->
      <main class="flex-grow-1 p-4">
        <router-view />
      </main>
    </div>

    <!-- Footer -->
    <footer class="bg-dark text-white text-center py-3 mt-auto">
      <p class="mb-0">&copy; 2025 財團法人台中市私立銀同碧湖陳氏社會福利基金會</p>
    </footer>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/stores/authStore'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const logout = () => {
  authStore.logout()
  router.push('/login')
}

const toggleAdminMenu = () => {
  // Placeholder for admin menu toggle
  console.log('Admin menu toggled')
}
</script>

<style scoped>
.min-vh-100 {
  min-height: 100vh;
}

.list-group-item {
  border: none;
  border-bottom: 1px solid #dee2e6;
}

.list-group-item strong {
  color: #495057;
}

.list-unstyled a {
  color: #495057;
  font-size: 0.9rem;
  transition: color 0.2s;
}

.list-unstyled a:hover {
  color: #007bff;
}

.list-unstyled a.router-link-active {
  color: #007bff;
  font-weight: 600;
}
</style>
