<template>
  <div class="form-view">
    <h1 class="mb-4">{{ isEditMode ? '編輯' : '新增' }}懷恩塔塔位</h1>

    <div class="card">
      <div class="card-body">
        <form @submit.prevent="handleSubmit">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label for="floor" class="form-label">樓層 *</label>
              <select class="form-select" id="floor" v-model="form.floor" required>
                <option value="1">1樓</option>
                <option value="2">2樓</option>
                <option value="3">3樓</option>
              </select>
            </div>

            <div class="col-md-6 mb-3">
              <label for="section" class="form-label">區域 *</label>
              <select class="form-select" id="section" v-model="form.section" required>
                <option v-for="sec in ['A','B','C','D','E','F']" :key="sec" :value="sec">
                  {{ sec }}區
                </option>
              </select>
            </div>
          </div>

          <div class="row">
            <div class="col-md-6 mb-3">
              <label for="row" class="form-label">排 *</label>
              <input 
                type="number" 
                class="form-control" 
                id="row"
                v-model.number="form.row"
                required
                min="1"
              />
            </div>

            <div class="col-md-6 mb-3">
              <label for="column" class="form-label">列 *</label>
              <input 
                type="number" 
                class="form-control" 
                id="column"
                v-model.number="form.column"
                required
                min="1"
              />
            </div>
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
            ></textarea>
          </div>

          <div v-if="error" class="alert alert-danger">{{ error }}</div>

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" :disabled="isLoading">
              {{ isLoading ? '處理中...' : '保存' }}
            </button>
            <router-link to="/kindness" class="btn btn-secondary">取消</router-link>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useKindnessStore } from '@/stores/kindnessStore'
import Swal from 'sweetalert2'

const route = useRoute()
const router = useRouter()
const kindnessStore = useKindnessStore()

const isEditMode = ref(false)
const isLoading = ref(false)
const error = ref('')

const form = reactive({
  floor: '1',
  section: 'A',
  row: 1,
  column: 1,
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
      const position = await kindnessStore.getById(positionId)
      form.floor = position.floor
      form.section = position.section
      form.row = position.row
      form.column = position.column
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
      await kindnessStore.update(route.params.id, form)
      Swal.fire('已更新!', '塔位資訊已更新', 'success')
    } else {
      await kindnessStore.create(form)
      Swal.fire('已建立!', '新塔位已建立', 'success')
    }
    router.push('/kindness')
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
