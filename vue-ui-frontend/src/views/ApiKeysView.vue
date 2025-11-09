<script setup lang="ts">
import { ref, onMounted } from 'vue'
import ApiKeyTable from '@/components/entities/api-keys/ApiKeyTable.vue'
import ApiKeyForm from '@/components/entities/api-keys/ApiKeyForm.vue'
import ApiKeyDisplayModal from '@/components/entities/api-keys/ApiKeyDisplayModal.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import Pagination from '@/components/common/Pagination.vue'
import { apiKeyService } from '@/services/apiKeyService'
import { useNotification } from '@/composables/useNotification'
import { usePagination } from '@/composables/usePagination'
import type { ApiKey, CreateApiKeyDto, UpdateApiKeyDto, ApiKeyCreationResponse } from '@/types/apiKey'

const apiKeys = ref<ApiKey[]>([])
const loading = ref(false)
const showForm = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedApiKey = ref<ApiKey | null>(null)
const operationLoading = ref(false)

// API Key display modal state
const showKeyDisplayModal = ref(false)
const newApiKeyValue = ref('')
const newApiKeyName = ref('')

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
} = usePagination(apiKeys, 20)

const loadApiKeys = async () => {
  loading.value = true
  try {
    const response = await apiKeyService.getAll()
    // Handle paginated response - extract items array
    apiKeys.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load API keys:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  formMode.value = 'create'
  selectedApiKey.value = null
  showForm.value = true
}

const handleEdit = (apiKey: ApiKey) => {
  formMode.value = 'edit'
  selectedApiKey.value = apiKey
  showForm.value = true
}

const handleDelete = async (apiKey: ApiKey) => {
  operationLoading.value = true
  try {
    await apiKeyService.delete(apiKey.id)
    await loadApiKeys()
    success('API key deleted successfully')
  } catch (err) {
    console.error('Failed to delete API key:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    operationLoading.value = false
  }
}

const handleFormSubmit = async (data: CreateApiKeyDto | UpdateApiKeyDto) => {
  operationLoading.value = true
  try {
    if (formMode.value === 'create') {
      const response: ApiKeyCreationResponse = await apiKeyService.create(data as CreateApiKeyDto)
      showForm.value = false
      
      // Show the API key display modal with the newly created key
      newApiKeyValue.value = response.key
      newApiKeyName.value = response.name
      showKeyDisplayModal.value = true
      
      await loadApiKeys()
      success('API key created successfully')
    } else if (selectedApiKey.value) {
      await apiKeyService.update(selectedApiKey.value.id, data as UpdateApiKeyDto)
      showForm.value = false
      await loadApiKeys()
      success('API key updated successfully')
    }
  } catch (err) {
    console.error('Failed to save API key:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    operationLoading.value = false
  }
}

const handleFormCancel = () => {
  showForm.value = false
  selectedApiKey.value = null
}

const handleKeyDisplayClose = () => {
  showKeyDisplayModal.value = false
  newApiKeyValue.value = ''
  newApiKeyName.value = ''
}

onMounted(() => {
  loadApiKeys()
})
</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>API Keys</h2>
      <button class="btn btn-create" @click="handleCreate" :disabled="operationLoading">
        Create API Key
      </button>
    </div>
    
    <LoadingSpinner v-if="operationLoading" />
    
    <ApiKeyTable
      :api-keys="paginatedItems"
      :loading="loading"
      :operation-loading="operationLoading"
      @edit="handleEdit"
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

    <ApiKeyForm
      v-if="showForm"
      :api-key="selectedApiKey"
      :mode="formMode"
      :loading="operationLoading"
      @submit="handleFormSubmit"
      @cancel="handleFormCancel"
    />

    <ApiKeyDisplayModal
      v-if="showKeyDisplayModal"
      :api-key-value="newApiKeyValue"
      :api-key-name="newApiKeyName"
      @close="handleKeyDisplayClose"
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
