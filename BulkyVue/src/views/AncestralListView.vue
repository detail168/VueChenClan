<template>
  <div class="ancestral-list-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>祖先牌位清單</h1>
      <div>
        <router-link to="/ancestral/new" class="btn btn-primary me-2">
          <i class="bi bi-plus-circle me-1"></i>新增牌位
        </router-link>
        <button class="btn btn-info me-2" @click="exportData">
          <i class="bi bi-download me-1"></i>匯出 Excel
        </button>
        <button class="btn btn-warning" @click="triggerImport">
          <i class="bi bi-upload me-1"></i>匯入 Excel
        </button>
        <input 
          ref="fileInput" 
          type="file" 
          accept=".xlsx,.xls" 
          style="display: none" 
          @change="importData"
        />
      </div>
    </div>

    <div v-if="ancestralStore.error" class="alert alert-danger alert-dismissible fade show">
      {{ ancestralStore.error }}
      <button type="button" class="btn-close" @click="ancestralStore.clearError()"></button>
    </div>

    <div class="card">
      <div class="card-body">
        <div v-if="ancestralStore.isLoading" class="text-center py-4">
          <div class="spinner-border" role="status">
            <span class="visually-hidden">載入中...</span>
          </div>
        </div>

        <div v-else>
          <table class="table table-hover">
            <thead class="table-light">
              <tr>
                <th>位置ID</th>
                <th>名字</th>
                <th>狀態</th>
                <th>建立日期</th>
                <th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="position in ancestralStore.positions" :key="position.id">
                <td class="fw-bold">{{ position.positionId }}</td>
                <td>{{ position.name }}</td>
                <td>
                  <span class="badge" :class="getStatusBadge(position.status)">
                    {{ position.status }}
                  </span>
                </td>
                <td>{{ formatDate(position.createdAt) }}</td>
                <td>
                  <router-link :to="`/ancestral/${position.id}/edit`" class="btn btn-sm btn-warning me-1">
                    編輯
                  </router-link>
                  <button class="btn btn-sm btn-danger" @click="deletePosition(position.id)">
                    刪除
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <div v-if="ancestralStore.positions.length === 0" class="text-center py-4 text-muted">
            無資料
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAncestralStore } from '@/stores/ancestralStore'
import Swal from 'sweetalert2'

const ancestralStore = useAncestralStore()
const fileInput = ref(null)

onMounted(async () => {
  await ancestralStore.fetchAll()
})

const getStatusBadge = (status) => {
  const badgeMap = {
    'Available': 'bg-success',
    'Occupied': 'bg-danger',
    'Reserved': 'bg-warning'
  }
  return badgeMap[status] || 'bg-secondary'
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-TW')
}

const deletePosition = async (id) => {
  const result = await Swal.fire({
    title: '確認刪除?',
    text: '此操作無法復原',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: '刪除',
    cancelButtonText: '取消'
  })

  if (result.isConfirmed) {
    try {
      await ancestralStore.deletePosition(id)
      Swal.fire('已刪除!', '', 'success')
    } catch (error) {
      Swal.fire('錯誤', error.message, 'error')
    }
  }
}

const exportData = async () => {
  try {
    await ancestralStore.exportExcel()
    Swal.fire('已匯出!', '檔案已下載', 'success')
  } catch (error) {
    Swal.fire('錯誤', error.message, 'error')
  }
}

const triggerImport = () => {
  fileInput.value.click()
}

const importData = async (event) => {
  const file = event.target.files?.[0]
  if (!file) return

  try {
    await ancestralStore.importExcel(file)
    Swal.fire('已匯入!', '資料匯入成功', 'success')
    fileInput.value.value = ''
  } catch (error) {
    Swal.fire('錯誤', error.message, 'error')
  }
}
</script>

<style scoped>
.ancestral-list-view {
  max-width: 1400px;
  margin: 0 auto;
}
</style>
