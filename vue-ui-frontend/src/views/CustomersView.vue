<script setup lang="ts">
import { ref, onMounted } from 'vue'
import CustomerTable from '@/components/entities/customers/CustomerTable.vue'
import CustomerForm from '@/components/entities/customers/CustomerForm.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import Pagination from '@/components/common/Pagination.vue'
import { customerService } from '@/services/customerService'
import { useNotification } from '@/composables/useNotification'
import { usePagination } from '@/composables/usePagination'
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer'

const customers = ref<Customer[]>([])
const loading = ref(false)
const showForm = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedCustomer = ref<Customer | null>(null)
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
} = usePagination(customers, 20)

const loadCustomers = async () => {
  loading.value = true
  try {
    const response = await customerService.getAll()
    // Handle paginated response - extract items array
    customers.value = Array.isArray(response) ? response : response.items || []
  } catch (err) {
    console.error('Failed to load customers:', err)
    // Error notification handled by global error handler
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  formMode.value = 'create'
  selectedCustomer.value = null
  showForm.value = true
}

const handleEdit = (customer: Customer) => {
  formMode.value = 'edit'
  selectedCustomer.value = customer
  showForm.value = true
}

const handleDelete = async (customer: Customer) => {
  operationLoading.value = true
  try {
    await customerService.delete(customer.id)
    await loadCustomers()
    success('Customer deleted successfully')
  } catch (err) {
    console.error('Failed to delete customer:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormSubmit = async (data: CreateCustomerDto | UpdateCustomerDto) => {
  operationLoading.value = true
  try {
    if (formMode.value === 'create') {
      await customerService.create(data as CreateCustomerDto)
      success('Customer created successfully')
    } else if (selectedCustomer.value) {
      await customerService.update(selectedCustomer.value.id, data as UpdateCustomerDto)
      success('Customer updated successfully')
    }
    showForm.value = false
    await loadCustomers()
  } catch (err) {
    console.error('Failed to save customer:', err)
    // Error notification handled by global error handler
  } finally {
    operationLoading.value = false
  }
}

const handleFormCancel = () => {
  showForm.value = false
  selectedCustomer.value = null
}

onMounted(() => {
  loadCustomers()
})
</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>Customers</h2>
      <button class="btn btn-create" @click="handleCreate" :disabled="operationLoading">
        Create Customer
      </button>
    </div>
    
    <LoadingSpinner v-if="operationLoading" />
    
    <CustomerTable
      :customers="paginatedItems"
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

    <CustomerForm
      v-if="showForm"
      :customer="selectedCustomer"
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
