<template>
  <div class="grid-view">
    <h1 class="mb-4">祖先牌位圖形檢視</h1>

    <div v-if="ancestralStore.config" class="card">
      <div class="card-body">
        <p class="text-muted">配置: {{ ancestralStore.config.side }} 側 × {{ ancestralStore.config.section }} 區 × {{ ancestralStore.config.level }} 層</p>

        <div class="grid-container">
          <!-- Left Side -->
          <div class="side-section">
            <h5 class="text-center mb-3">左側 (L側)</h5>
            <div v-for="section in ['甲', '乙', '丙', '丁']" :key="`l-${section}`" class="section-grid mb-4">
              <h6 class="text-center mb-2">{{ section }}區</h6>
              <div class="grid">
                <div 
                  v-for="level in 10" 
                  :key="`l-${section}-${level}`"
                  class="grid-cell"
                  :class="getPositionClass(`L側-${section}區-${level}`)"
                  @click="selectPosition(`L側-${section}區-${level}`)"
                >
                  {{ level }}
                </div>
              </div>
            </div>
          </div>

          <!-- Right Side -->
          <div class="side-section">
            <h5 class="text-center mb-3">右側 (R側)</h5>
            <div v-for="section in ['甲', '乙', '丙', '丁']" :key="`r-${section}`" class="section-grid mb-4">
              <h6 class="text-center mb-2">{{ section }}區</h6>
              <div class="grid">
                <div 
                  v-for="level in 10" 
                  :key="`r-${section}-${level}`"
                  class="grid-cell"
                  :class="getPositionClass(`R側-${section}區-${level}`)"
                  @click="selectPosition(`R側-${section}區-${level}`)"
                >
                  {{ level }}
                </div>
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
import { computed, onMounted } from 'vue'
import { useAncestralStore } from '@/stores/ancestralStore'

const ancestralStore = useAncestralStore()

const occupiedPositions = computed(() => {
  return ancestralStore.positions
    .filter(p => p.status === 'Occupied')
    .map(p => p.positionId)
})

onMounted(async () => {
  await ancestralStore.fetchConfig()
  await ancestralStore.fetchAll()
})

const getPositionClass = (positionId) => {
  if (occupiedPositions.value.includes(positionId)) {
    return 'occupied'
  }
  return 'available'
}

const selectPosition = (positionId) => {
  const position = ancestralStore.positions.find(p => p.positionId === positionId)
  if (position) {
    // Handle position click
    console.log('Position selected:', position)
  }
}
</script>

<style scoped>
.grid-view {
  max-width: 1200px;
  margin: 0 auto;
}

.grid-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-top: 2rem;
}

.side-section {
  border: 1px solid #ddd;
  padding: 1rem;
  border-radius: 8px;
}

.section-grid {
  background: #f9f9f9;
  padding: 1rem;
  border-radius: 4px;
}

.grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 0.5rem;
}

.grid-cell {
  aspect-ratio: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid #ddd;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  transition: all 0.2s;
  background: white;
}

.grid-cell.available:hover {
  border-color: #28a745;
  background: #d4edda;
  transform: scale(1.05);
}

.grid-cell.occupied {
  background: #f8d7da;
  border-color: #dc3545;
  cursor: not-allowed;
}

.grid-cell.occupied:hover {
  background: #f5c6cb;
}

@media (max-width: 768px) {
  .grid-container {
    grid-template-columns: 1fr;
  }
}
</style>
