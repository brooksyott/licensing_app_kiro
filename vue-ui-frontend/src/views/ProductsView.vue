<script setup lang="ts">
import { ref, onMounted } from 'vue'
import ProductTable from '@/components/entities/products/ProductTable.vue'
import ProductForm from '@/components/entities/products/ProductForm.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import Pagination from '@/components/common/Pagination.vue'
import { productService } from '@/services/productService'
import { useNotification } from '@/composables/useNotification'
import { usePagination } from '@/composables/usePagination'
import type { Product, CreateProductDto, UpdateProductDto } from '@/types/product'

const products = ref<Product[]>([])
const loading = ref(false)
const showForm = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedProduct = ref<Product | null>(null)
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
} = usePagination(products, 20)

const loadProducts = async () => {
  loading.value = true
  try {
    const response = await productService.getAll()
    // Handle paginated response - extract items array
    products.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load products:', err)
    // Error notification handled by global error handler
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  formMode.value = 'create'
  selectedProduct.value = null
  showForm.value = true
}

const handleEdit = (product: Product) => {
  formMode.value = 'edit'
  selectedProduct.value = product
  showForm.value = true
}

const handleDelete = async (product: Product) => {
  operationLoading.value = true
  try {
    await productService.delete(product.id)
    await loadProducts()
    success('Product deleted successfully')
  } catch (err) {
    console.error('Failed to delete product:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormSubmit = async (data: CreateProductDto | UpdateProductDto) => {
  operationLoading.value = true
  try {
    if (formMode.value === 'create') {
      await productService.create(data as CreateProductDto)
      success('Product created successfully')
    } else if (selectedProduct.value) {
      await productService.update(selectedProduct.value.id, data as UpdateProductDto)
      success('Product updated successfully')
    }
    showForm.value = false
    await loadProducts()
  } catch (err) {
    console.error('Failed to save product:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormCancel = () => {
  showForm.value = false
  selectedProduct.value = null
}

onMounted(() => {
  loadProducts()
})
</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>Products</h2>
      <button class="btn btn-create" @click="handleCreate" :disabled="operationLoading">
        Create Product
      </button>
    </div>
    
    <LoadingSpinner v-if="operationLoading" />
    
    <ProductTable
      :products="paginatedItems"
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

    <ProductForm
      v-if="showForm"
      :product="selectedProduct"
      :mode="formMode"
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
