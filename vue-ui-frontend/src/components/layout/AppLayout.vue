<script setup lang="ts">
import AppHeader from './AppHeader.vue'
import AppFooter from './AppFooter.vue'
</script>

<template>
  <div class="app-layout">
    <AppHeader />
    <main class="main-content">
      <router-view />
    </main>
    <AppFooter />
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  padding: 2rem;
  max-width: 1400px;
  width: 100%;
  margin: 0 auto;
}
</style>
