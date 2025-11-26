<template>
  <div class="form-view">
    <h1 class="mb-4">{{ isEditMode ? '編輯' : '新增' }}祖先牌位</h1>

    <div class="card">
      <div class="card-body">
        <form @submit.prevent="handleSubmit">
          <div class="mb-3">
            <label for="positionId" class="form-label">位置ID *</label>
            <input 
              type="text" 
              class="form-control" 
              id="positionId"
              v-model="form.positionId"
              required
              placeholder="例：L側-甲區-1:001"
            />
          </div>

          <div class="mb-3">
            <label for="name" class="form-label">名字 *</label>
            <input 
              type="text" 
              class="form-control" 
              id="name"
              v-model="form.name"
              required
              placeholder="祖先名字"
            />
          </div>

          <div class="mb-3">
            <label for="status" class="form-label">狀態 *</label>
            <select class="form-select" id="status" v-model="form.status" required>
              <option value="Available">可用</option>
              <option value="Occupied">已佔用</option>
              <option value="Reserved">已預留</option>
            </select>
          </div>

          <div class="mb-3">
            <label for="note" class="form-label">備註</label>
            <textarea 
              class="form-control" 
              id="note"
              v-model="form.note"
              rows="3"
              placeholder="額外資訊或備註"
            ></textarea>
          </div>

          <div v-if="error" class="alert alert-danger">{{ error }}</div>

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" :disabled="isLoading">
              {{ isLoading ? '處理中...' : '保存' }}
            </button>
            <router-link to="/ancestral" class="btn btn-secondary">取消</router-link>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAncestralStore } from '@/stores/ancestralStore'
import Swal from 'sweetalert2'

const route = useRoute()
const router = useRouter()
const ancestralStore = useAncestralStore()

const isEditMode = ref(false)
const isLoading = ref(false)
const error = ref('')

const form = reactive({
  positionId: '',
  name: '',
  status: 'Available',
  note: ''
})

onMounted(async () => {
  const positionId = route.params.id
  if (positionId) {
    isEditMode.value = true
    isLoading.value = true
    try {
      const position = await ancestralStore.getById(positionId)
      form.positionId = position.positionId
      form.name = position.name
      form.status = position.status
      form.note = position.note || ''
    } catch (err) {
      error.value = err.message
    } finally {
      isLoading.value = false
    }
  }
})

const handleSubmit = async () => {
  isLoading.value = true
  error.value = ''

  try {
    if (isEditMode.value) {
      await ancestralStore.update(route.params.id, form)
      Swal.fire('已更新!', '牌位資訊已更新', 'success')
    } else {
      await ancestralStore.create(form)
      Swal.fire('已建立!', '新牌位已建立', 'success')
    }
    router.push('/ancestral')
  } catch (err) {
    error.value = err.message
    Swal.fire('錯誤', err.message, 'error')
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.form-view {
  max-width: 600px;
  margin: 0 auto;
}
</style>
