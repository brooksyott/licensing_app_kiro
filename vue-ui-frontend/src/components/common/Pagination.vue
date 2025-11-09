<script setup lang="ts">
interface Props {
  currentPage: number
  totalPages: number
}

interface Emits {
  (e: 'page-change', page: number): void
  (e: 'next'): void
  (e: 'previous'): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()

const handleNext = () => {
  emit('next')
}

const handlePrevious = () => {
  emit('previous')
}
</script>

<template>
  <div class="pagination">
    <button
      class="pagination-btn"
      :disabled="currentPage === 1"
      @click="handlePrevious"
    >
      Previous
    </button>
    
    <span class="pagination-info">
      Page {{ currentPage }} of {{ totalPages }}
    </span>
    
    <button
      class="pagination-btn"
      :disabled="currentPage === totalPages"
      @click="handleNext"
    >
      Next
    </button>
  </div>
</template>

<style scoped>
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem;
  background: white;
  border-radius: 0 0 8px 8px;
}

.pagination-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #f8f9fa;
  border-color: #3498db;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  color: #2c3e50;
  font-size: 0.9rem;
}
</style>
