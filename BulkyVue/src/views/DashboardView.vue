<template>
  <div class="dashboard-view">
    <h1 class="mb-4">儀表板</h1>
    
    <div class="row">
      <div class="col-md-6 mb-3">
        <div class="card shadow-sm">
          <div class="card-body">
            <h5 class="card-title">祖先牌位</h5>
            <p class="card-text">
              <span class="badge bg-primary">{{ ancestralStore.totalCount }}</span>
              位置總計
            </p>
            <p class="card-text text-success">
              <span class="badge bg-success">{{ ancestralStore.availablePositions.length }}</span>
              可用位置
            </p>
            <router-link to="/ancestral" class="btn btn-sm btn-primary">
              查看詳情
            </router-link>
          </div>
        </div>
      </div>

      <div class="col-md-6 mb-3">
        <div class="card shadow-sm">
          <div class="card-body">
            <h5 class="card-title">懷恩塔</h5>
            <p class="card-text">
              <span class="badge bg-primary">{{ kindnessStore.totalCount }}</span>
              位置總計
            </p>
            <p class="card-text text-success">
              <span class="badge bg-success">{{ kindnessStore.availablePositions.length }}</span>
              可用位置
            </p>
            <router-link to="/kindness" class="btn btn-sm btn-primary">
              查看詳情
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-body">
        <h5 class="card-title">系統資訊</h5>
        <table class="table table-sm">
          <tr>
            <td><strong>使用者：</strong></td>
            <td>{{ authStore.user?.email }}</td>
          </tr>
          <tr>
            <td><strong>角色：</strong></td>
            <td>
              <span v-for="role in authStore.userRoles" :key="role" class="badge bg-info me-1">
                {{ role }}
              </span>
            </td>
          </tr>
          <tr>
            <td><strong>登入時間：</strong></td>
            <td>{{ new Date().toLocaleString() }}</td>
          </tr>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useAncestralStore } from '@/stores/ancestralStore'
import { useKindnessStore } from '@/stores/kindnessStore'

const authStore = useAuthStore()
const ancestralStore = useAncestralStore()
const kindnessStore = useKindnessStore()

onMounted(async () => {
  await ancestralStore.fetchAll()
  await kindnessStore.fetchAll()
})
</script>

<style scoped>
.dashboard-view {
  max-width: 1200px;
  margin: 0 auto;
}

.card {
  transition: transform 0.2s, box-shadow 0.2s;
}

.card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1) !important;
}
</style>
