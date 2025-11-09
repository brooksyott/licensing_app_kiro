<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import NotificationContainer from '@/components/common/NotificationContainer.vue'
import { useNotification } from '@/composables/useNotification'

const { error } = useNotification()

// Listen for API error events and display error notifications
const handleApiError = (event: Event) => {
  const customEvent = event as CustomEvent
  const { message } = customEvent.detail
  
  // The error message is already formatted by the error handler utility
  error(message)
}

onMounted(() => {
  window.addEventListener('api-error', handleApiError)
})

onUnmounted(() => {
  window.removeEventListener('api-error', handleApiError)
})
</script>

<template>
  <div id="app">
    <NotificationContainer />
    <router-view />
  </div>
</template>

<style>
/* Global styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
</style>
