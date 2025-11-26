<template>
  <div class="grid-view">
    <h1 class="mb-4">懷恩塔圖形檢視</h1>

    <div v-if="kindnessStore.config" class="card">
      <div class="card-body">
        <p class="text-muted">配置: {{ kindnessStore.config.floor }} 樓 × {{ kindnessStore.config.section }} 區</p>

        <!-- Floor Tabs -->
        <ul class="nav nav-tabs mb-3">
          <li class="nav-item">
            <a :class="['nav-link', { active: selectedFloor === '1' }]" href="#" @click.prevent="selectedFloor = '1'">
              1樓
            </a>
          </li>
          <li class="nav-item">
            <a :class="['nav-link', { active: selectedFloor === '2' }]" href="#" @click.prevent="selectedFloor = '2'">
              2樓
            </a>
          </li>
          <li class="nav-item">
            <a :class="['nav-link', { active: selectedFloor === '3' }]" href="#" @click.prevent="selectedFloor = '3'">
              3樓
            </a>
          </li>
        </ul>

        <!-- Floor Content -->
        <div class="floor-sections">
          <div v-for="section in ['A','B','C','D','E','F']" :key="`f${selectedFloor}-${section}`" class="section mb-4">
            <h6 class="text-center mb-2">{{ section }}區</h6>
            <div class="grid">
              <div 
                v-for="pos in getPositionsForSection(selectedFloor, section)" 
                :key="pos"
                class="grid-cell"
                :class="getPositionClass(pos)"
                @click="selectPosition(pos)"
              >
                {{ pos }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="alert alert-info">
      配置載入中...
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useKindnessStore } from '@/stores/kindnessStore'

const kindnessStore = useKindnessStore()
const selectedFloor = ref('1')

const occupiedPositions = computed(() => {
  return kindnessStore.positions
    .filter(p => p.status === 'Occupied')
    .map(p => p.positionId)
})

onMounted(async () => {
  await kindnessStore.fetchConfig()
  await kindnessStore.fetchAll()
})

const getPositionsForSection = (floor, section) => {
  const max = floor === '3' ? 7 : 4
  return Array.from({ length: max }, (_, i) => `F${floor}${section}-${i + 1}-1`)
}

const getPositionClass = (positionId) => {
  if (occupiedPositions.value.includes(positionId)) {
    return 'occupied'
  }
  return 'available'
}

const selectPosition = (positionId) => {
  const position = kindnessStore.positions.find(p => p.positionId === positionId)
  if (position) {
    console.log('Position selected:', position)
  }
}
</script>

<style scoped>
.grid-view {
  max-width: 1200px;
  margin: 0 auto;
}

.floor-sections {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.section {
  background: #f9f9f9;
  padding: 1rem;
  border-radius: 4px;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(40px, 1fr));
  gap: 0.3rem;
}

.grid-cell {
  aspect-ratio: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.8rem;
  font-weight: bold;
  transition: all 0.2s;
  background: white;
}

.grid-cell.available:hover {
  border-color: #28a745;
  background: #d4edda;
  transform: scale(1.1);
}

.grid-cell.occupied {
  background: #f8d7da;
  border-color: #dc3545;
  cursor: not-allowed;
}

.grid-cell.occupied:hover {
  background: #f5c6cb;
}
</style>
