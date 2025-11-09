<script setup lang="ts">
import { ref, onMounted } from 'vue'
import LicenseTable from '@/components/entities/licenses/LicenseTable.vue'
import LicenseForm from '@/components/entities/licenses/LicenseForm.vue'
import LicenseViewModal from '@/components/entities/licenses/LicenseViewModal.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import Pagination from '@/components/common/Pagination.vue'
import { licenseService } from '@/services/licenseService'
import { customerService } from '@/services/customerService'
import { productService } from '@/services/productService'
import { skuService } from '@/services/skuService'
import { rsaKeyService } from '@/services/rsaKeyService'
import { useNotification } from '@/composables/useNotification'
import { usePagination } from '@/composables/usePagination'
import type { License, CreateLicenseDto, UpdateLicenseDto } from '@/types/license'
import type { Customer } from '@/types/customer'
import type { Product } from '@/types/product'
import type { Sku } from '@/types/sku'
import type { RsaKey } from '@/types/rsaKey'

const licenses = ref<License[]>([])
const customers = ref<Customer[]>([])
const products = ref<Product[]>([])
const skus = ref<Sku[]>([])
const rsaKeys = ref<RsaKey[]>([])
const loading = ref(false)
const showForm = ref(false)
const showViewModal = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedLicense = ref<License | null>(null)
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
} = usePagination(licenses, 20)

const loadLicenses = async () => {
  loading.value = true
  try {
    const response = await licenseService.getAll()
    // Handle paginated response - extract items array
    licenses.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load licenses:', err)
    // Error notification handled by global error handler
  } finally {
    loading.value = false
  }
}

const loadRelatedEntities = async () => {
  try {
    const [customersData, productsData, skusData, rsaKeysData] = await Promise.all([
      customerService.getAll(),
      productService.getAll(),
      skuService.getAll(),
      rsaKeyService.getAll()
    ])
    // Handle paginated responses - extract items arrays
    customers.value = Array.isArray(customersData) ? customersData : customersData.items || []
    products.value = Array.isArray(productsData) ? productsData : productsData.items || []
    skus.value = Array.isArray(skusData) ? skusData : skusData.items || []
    rsaKeys.value = Array.isArray(rsaKeysData) ? rsaKeysData : rsaKeysData.items || []
  } catch (err) {
    console.error('Failed to load related entities:', err)
    // Error notification handled by global error handler with specific message
  }
}

const handleCreate = () => {
  formMode.value = 'create'
  selectedLicense.value = null
  showForm.value = true
}

const handleView = (license: License) => {
  selectedLicense.value = license
  showViewModal.value = true
}

const handleEdit = (license: License) => {
  formMode.value = 'edit'
  selectedLicense.value = license
  showForm.value = true
}

const handleDelete = async (license: License) => {
  operationLoading.value = true
  try {
    await licenseService.delete(license.id)
    await loadLicenses()
    success('License deleted successfully')
  } catch (err) {
    console.error('Failed to delete license:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormSubmit = async (data: CreateLicenseDto | UpdateLicenseDto) => {
  operationLoading.value = true
  try {
    if (formMode.value === 'create') {
      await licenseService.create(data as CreateLicenseDto)
      success('License created successfully')
    } else if (selectedLicense.value) {
      await licenseService.update(selectedLicense.value.id, data as UpdateLicenseDto)
      success('License updated successfully')
    }
    showForm.value = false
    await loadLicenses()
  } catch (err) {
    console.error('Failed to save license:', err)
    // Error notification handled by global error handler with specific message
  } finally {
    operationLoading.value = false
  }
}

const handleFormCancel = () => {
  showForm.value = false
  selectedLicense.value = null
}

const handleViewModalClose = () => {
  showViewModal.value = false
  selectedLicense.value = null
}

onMounted(async () => {
  await loadRelatedEntities()
  await loadLicenses()
})
</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>Licenses</h2>
      <button class="btn btn-create" @click="handleCreate" :disabled="operationLoading">
        Create License
      </button>
    </div>
    
    <LoadingSpinner v-if="operationLoading" />
    
    <LicenseTable
      :licenses="paginatedItems"
      :loading="loading"
      :operation-loading="operationLoading"
      @view="handleView"
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

    <LicenseForm
      v-if="showForm"
      :license="selectedLicense"
      :mode="formMode"
      :customers="customers"
      :products="products"
      :skus="skus"
      :rsa-keys="rsaKeys"
      :loading="operationLoading"
      @submit="handleFormSubmit"
      @cancel="handleFormCancel"
    />

    <LicenseViewModal
      v-if="showViewModal && selectedLicense"
      :license="selectedLicense"
      @close="handleViewModalClose"
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
