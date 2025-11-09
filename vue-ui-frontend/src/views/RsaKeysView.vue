<script setup lang="ts">
import { ref, onMounted } from 'vue'
import RsaKeyTable from '@/components/entities/rsa-keys/RsaKeyTable.vue'
import RsaKeyForm from '@/components/entities/rsa-keys/RsaKeyForm.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import Pagination from '@/components/common/Pagination.vue'
import { rsaKeyService } from '@/services/rsaKeyService'
import { useNotification } from '@/composables/useNotification'
import { usePagination } from '@/composables/usePagination'
import type { RsaKey, CreateRsaKeyDto } from '@/types/rsaKey'

const rsaKeys = ref<RsaKey[]>([])
const loading = ref(false)
const showForm = ref(false)
const operationLoading = ref(false)

const { success } = useNotification()

// Pagination
const {
  paginatedItems,
  showPagination,
  currentPage,
  totalPages,
  nextPage,
  previousPage,
  goToPage
} = usePagination(rsaKeys, 20)

const loadRsaKeys = async () => {
  loading.value = true
  try {
    const response = await rsaKeyService.getAll()
    // Handle paginated response - extract items array
    rsaKeys.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load RSA keys:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  showForm.value = true
}

const handleDelete = async (rsaKey: RsaKey) => {
  operationLoading.value = true
  try {
    await rsaKeyService.delete(rsaKey.id)
    await loadRsaKeys()
    success('RSA key deleted successfully')
  } catch (err) {
    console.error('Failed to delete RSA key:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    operationLoading.value = false
  }
}

const handleFormSubmit = async (data: CreateRsaKeyDto) => {
  operationLoading.value = true
  try {
    await rsaKeyService.create(data)
    success('RSA key created successfully')
    showForm.value = false
    await loadRsaKeys()
  } catch (err) {
    console.error('Failed to create RSA key:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    operationLoading.value = false
  }
}

const handleFormCancel = () => {
  showForm.value = false
}

onMounted(() => {
  loadRsaKeys()
})

</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>RSA Keys</h2>
      <button class="btn btn-create" @click="handleCreate" :disabled="operationLoading">
        Create RSA Key
      </button>
    </div>
    
    <LoadingSpinner v-if="operationLoading" />
    
    <RsaKeyTable
      :rsa-keys="paginatedItems"
      :loading="loading"
      :operation-loading="operationLoading"
      @delete="handleDelete"
    />

    <Pagination
      v-if="showPagination"
      :current-page="currentPage"
      :total-pages="totalPages"
      @page-change="goToPage"
      @next="nextPage"
      @previous="previousPage"
    />

    <RsaKeyForm
      v-if="showForm"
      :loading="operationLoading"
      @submit="handleFormSubmit"
      @cancel="handleFormCancel"
    />
  </div>
</template>

<style scoped>
.view-container {
  padding: 1rem;
}

.view-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.view-header h2 {
  color: #2c3e50;
  margin: 0;
}

.btn-create {
  background-color: #27ae60;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.btn-create:hover:not(:disabled) {
  background-color: #229954;
}

.btn-create:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
