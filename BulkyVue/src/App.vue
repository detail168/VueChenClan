<template>
  <div id="app">
    <router-view />
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
  // Load configuration on app start
  await ancestralStore.fetchConfig()
  await kindnessStore.fetchConfig()
})
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background-color: #f8f9fa;
}

#app {
  min-height: 100vh;
}
</style>
