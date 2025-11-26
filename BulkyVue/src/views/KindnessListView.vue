<template>
  <div class="kindness-list-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>懷恩塔清單</h1>
      <div>
        <router-link to="/kindness/new" class="btn btn-primary me-2">
          <i class="bi bi-plus-circle me-1"></i>新增塔位
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

    <!-- Filters -->
    <div class="card mb-3">
      <div class="card-body">
        <div class="row">
          <div class="col-md-3">
            <select class="form-select" v-model="selectedFloor" @change="applyFilters">
              <option value="">全部樓層</option>
              <option value="1">1樓</option>
              <option value="2">2樓</option>
              <option value="3">3樓</option>
            </select>
          </div>
          <div class="col-md-3">
            <select class="form-select" v-model="selectedSection" @change="applyFilters">
              <option value="">全部區域</option>
              <option v-for="sec in ['A','B','C','D','E','F']" :key="sec" :value="sec">
                {{ sec }}區
              </option>
            </select>
          </div>
          <div class="col-md-3">
            <select class="form-select" v-model="selectedStatus" @change="applyFilters">
              <option value="">全部狀態</option>
              <option value="Available">可用</option>
              <option value="Occupied">已佔用</option>
              <option value="Reserved">已預留</option>
            </select>
          </div>
          <div class="col-md-3">
            <button class="btn btn-secondary w-100" @click="clearFilters">清除篩選</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="kindnessStore.error" class="alert alert-danger alert-dismissible fade show">
      {{ kindnessStore.error }}
      <button type="button" class="btn-close" @click="kindnessStore.clearError()"></button>
    </div>

    <div class="card">
      <div class="card-body">
        <div v-if="kindnessStore.isLoading" class="text-center py-4">
          <div class="spinner-border" role="status">
            <span class="visually-hidden">載入中...</span>
          </div>
        </div>

        <div v-else>
          <table class="table table-hover">
            <thead class="table-light">
              <tr>
                <th>位置ID</th>
                <th>樓層</th>
                <th>區域</th>
                <th>名字</th>
                <th>狀態</th>
                <th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="position in kindnessStore.filteredPositions" :key="position.id">
                <td class="fw-bold">{{ position.positionId }}</td>
                <td>{{ position.floor }}樓</td>
                <td>{{ position.section }}區</td>
                <td>{{ position.name }}</td>
                <td>
                  <span class="badge" :class="getStatusBadge(position.status)">
                    {{ position.status }}
                  </span>
                </td>
                <td>
                  <router-link :to="`/kindness/${position.id}/edit`" class="btn btn-sm btn-warning me-1">
                    編輯
                  </router-link>
                  <button class="btn btn-sm btn-danger" @click="deletePosition(position.id)">
                    刪除
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <div v-if="kindnessStore.filteredPositions.length === 0" class="text-center py-4 text-muted">
            無資料
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useKindnessStore } from '@/stores/kindnessStore'
import Swal from 'sweetalert2'

const kindnessStore = useKindnessStore()
const fileInput = ref(null)
const selectedFloor = ref('')
const selectedSection = ref('')
const selectedStatus = ref('')

onMounted(async () => {
  await kindnessStore.fetchAll()
})

const getStatusBadge = (status) => {
  const badgeMap = {
    'Available': 'bg-success',
    'Occupied': 'bg-danger',
    'Reserved': 'bg-warning'
  }
  return badgeMap[status] || 'bg-secondary'
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
      await kindnessStore.deletePosition(id)
      Swal.fire('已刪除!', '', 'success')
    } catch (error) {
      Swal.fire('錯誤', error.message, 'error')
    }
  }
}

const applyFilters = () => {
  if (selectedFloor.value) kindnessStore.setFilter('floor', selectedFloor.value)
  if (selectedSection.value) kindnessStore.setFilter('section', selectedSection.value)
  if (selectedStatus.value) kindnessStore.setFilter('status', selectedStatus.value)
}

const clearFilters = () => {
  selectedFloor.value = ''
  selectedSection.value = ''
  selectedStatus.value = ''
  kindnessStore.clearFilters()
}

const exportData = async () => {
  try {
    await kindnessStore.exportExcel()
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
    await kindnessStore.importExcel(file)
    Swal.fire('已匯入!', '資料匯入成功', 'success')
    fileInput.value.value = ''
  } catch (error) {
    Swal.fire('錯誤', error.message, 'error')
  }
}
</script>

<style scoped>
.kindness-list-view {
  max-width: 1400px;
  margin: 0 auto;
}
</style>
