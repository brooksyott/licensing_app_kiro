<script setup lang="ts">
import { ref, onMounted } from 'vue'
import SkuTable from '@/components/entities/skus/SkuTable.vue'
import SkuForm from '@/components/entities/skus/SkuForm.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import Pagination from '@/components/common/Pagination.vue'
import { skuService } from '@/services/skuService'
import { productService } from '@/services/productService'
import { useNotification } from '@/composables/useNotification'
import { usePagination } from '@/composables/usePagination'
import type { Sku, CreateSkuDto, UpdateSkuDto } from '@/types/sku'
import type { Product } from '@/types/product'

const skus = ref<Sku[]>([])
const products = ref<Product[]>([])
const loading = ref(false)
const showForm = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedSku = ref<Sku | null>(null)
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
} = usePagination(skus, 20)

const loadSkus = async () => {
  loading.value = true
  try {
    const response = await skuService.getAll()
    // Handle paginated response - extract items array
    skus.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load SKUs:', err)
    // Error notification handled by global error handler
  } finally {
    loading.value = false
  }
}

const loadProducts = async () => {
  try {
    const response = await productService.getAll()
    // Handle paginated response - extract items array
    products.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load products:', err)
    // Error notification handled by global error handler
  }
}

const handleCreate = () => {
  formMode.value = 'create'
  selectedSku.value = null
  showForm.value = true
}

const handleEdit = (sku: Sku) => {
  formMode.value = 'edit'
  selectedSku.value = sku
  showForm.value = true
}

const handleDelete = async (sku: Sku) => {
  operationLoading.value = true
  try {
    await skuService.delete(sku.id)
    await loadSkus()
    success('SKU deleted successfully')
  } catch (err) {
    console.error('Failed to delete SKU:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormSubmit = async (data: CreateSkuDto | UpdateSkuDto) => {
  operationLoading.value = true
  try {
    if (formMode.value === 'create') {
      await skuService.create(data as CreateSkuDto)
      success('SKU created successfully')
    } else if (selectedSku.value) {
      await skuService.update(selectedSku.value.id, data as UpdateSkuDto)
      success('SKU updated successfully')
    }
    showForm.value = false
    await loadSkus()
  } catch (err) {
    console.error('Failed to save SKU:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormCancel = () => {
  showForm.value = false
  selectedSku.value = null
}

onMounted(async () => {
  await loadProducts()
  await loadSkus()
})
</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>SKUs</h2>
      <button class="btn btn-create" @click="handleCreate" :disabled="operationLoading">
        Create SKU
      </button>
    </div>
    
    <LoadingSpinner v-if="operationLoading" />
    
    <SkuTable
      :skus="paginatedItems"
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

    <SkuForm
      v-if="showForm"
      :sku="selectedSku"
      :mode="formMode"
      :products="products"
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
