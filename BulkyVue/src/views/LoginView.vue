<template>
  <div class="login-container">
    <div class="login-card">
      <h1 class="text-center mb-4">BulkyBook</h1>
      <h3 class="text-center text-muted mb-4">位置管理系統</h3>
      
      <form @submit.prevent="handleLogin">
        <div class="mb-3">
          <label for="email" class="form-label">電子郵件</label>
          <input 
            type="email" 
            class="form-control" 
            id="email"
            v-model="email"
            required
            placeholder="your@email.com"
          />
        </div>
        
        <div class="mb-3">
          <label for="password" class="form-label">密碼</label>
          <input 
            type="password" 
            class="form-control" 
            id="password"
            v-model="password"
            required
            placeholder="••••••••"
          />
        </div>
        
        <div v-if="error" class="alert alert-danger" role="alert">
          {{ error }}
        </div>
        
        <button type="submit" class="btn btn-primary w-100" :disabled="isLoading">
          <span v-if="isLoading">
            <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
            登入中...
          </span>
          <span v-else>登入</span>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useRouter, useRoute } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()

const email = ref('')
const password = ref('')
const error = ref('')
const isLoading = ref(false)

const handleLogin = async () => {
  error.value = ''
  isLoading.value = true

  try {
    await authStore.login(email.value, password.value)
    const redirectPath = route.query.redirect || '/'
    router.push(redirectPath)
  } catch (err) {
    error.value = err.message
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.1);
  padding: 2rem;
  width: 100%;
  max-width: 400px;
}

.login-card h1 {
  color: #667eea;
  font-weight: 700;
}

.login-card h3 {
  font-size: 1rem;
}
</style>
