<script setup lang="ts">
import type { License } from '@/types/license'

interface Props {
  licenses: License[]
  loading: boolean
  operationLoading?: boolean
}

interface Emits {
  (e: 'view', license: License): void
  (e: 'edit', license: License): void
  (e: 'delete', license: License): void
}

withDefaults(defineProps<Props>(), {
  operationLoading: false
})
const emit = defineEmits<Emits>()

const handleView = (license: License) => {
  emit('view', license)
}

const handleEdit = (license: License) => {
  emit('edit', license)
}

const handleDelete = (license: License) => {
  if (confirm(`Are you sure you want to delete license for "${license.customerName}"?`)) {
    emit('delete', license)
  }
}

const formatDate = (date: string | null) => {
  if (!date) return 'N/A'
  return new Date(date).toLocaleDateString()
}
</script>

<template>
  <div class="table-container">
    <div v-if="loading" class="loading">Loading licenses...</div>
    <table v-else class="data-table">
      <thead>
        <tr>
          <th>Customer Name</th>
          <th>Product Name</th>
          <th>SKUs</th>
          <th>License Type</th>
          <th>Status</th>
          <th>Expiration Date</th>
          <th>Activations</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="licenses.length === 0">
          <td colspan="8" class="no-data">No licenses found</td>
        </tr>
        <tr 
          v-for="license in licenses" 
          :key="license.id"
          v-memo="[license.id, license.status, license.currentActivations, operationLoading]"
        >
          <td>{{ license.customerName }}</td>
          <td>{{ license.productName }}</td>
          <td>
            <div class="sku-list">
              <span 
                v-for="(sku, index) in license.skus.slice(0, 2)" 
                :key="sku.skuId"
                class="sku-badge"
              >
                {{ sku.skuName }}
              </span>
              <span 
                v-if="license.skus.length > 2" 
                class="sku-count"
              >
                +{{ license.skus.length - 2 }} more
              </span>
              <span v-if="license.skus.length === 0" class="no-skus">
                No SKUs
              </span>
            </div>
          </td>
          <td>{{ license.licenseType }}</td>
          <td>
            <span :class="['status-badge', license.status.toLowerCase()]">
              {{ license.status }}
            </span>
          </td>
          <td>{{ formatDate(license.expirationDate) }}</td>
          <td>{{ license.currentActivations }} / {{ license.maxActivations }}</td>
          <td class="actions">
            <button class="btn btn-view" @click="handleView(license)" :disabled="operationLoading">View</button>
            <button class="btn btn-edit" @click="handleEdit(license)" :disabled="operationLoading">Edit</button>
            <button class="btn btn-delete" @click="handleDelete(license)" :disabled="operationLoading">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.table-container {
  overflow-x: auto;
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.loading {
  padding: 2rem;
  text-align: center;
  color: #7f8c8d;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 800px;
}

.data-table th {
  background-color: #f8f9fa;
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #2c3e50;
  border-bottom: 2px solid #e9ecef;
}

.data-table td {
  padding: 1rem;
  border-bottom: 1px solid #e9ecef;
  color: #495057;
  text-align: left;
}

.data-table tbody tr:hover {
  background-color: #f8f9fa;
}

.no-data {
  text-align: center;
  color: #7f8c8d;
  font-style: italic;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 500;
}

.status-badge.active {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.expired {
  background-color: #f8d7da;
  color: #721c24;
}

.status-badge.suspended {
  background-color: #fff3cd;
  color: #856404;
}

.sku-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  align-items: center;
}

.sku-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background-color: #e3f2fd;
  color: #1565c0;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 500;
  white-space: nowrap;
}

.sku-count {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background-color: #f5f5f5;
  color: #616161;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 500;
  white-space: nowrap;
}

.no-skus {
  color: #9e9e9e;
  font-style: italic;
  font-size: 0.85rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-view {
  background-color: #3498db;
  color: white;
}

.btn-view:hover:not(:disabled) {
  background-color: #2980b9;
}

.btn-edit {
  background-color: #00A3AD;
  color: white;
}

.btn-edit:hover:not(:disabled) {
  background-color: #008A93;
}

.btn-delete {
  background-color: #e74c3c;
  color: white;
}

.btn-delete:hover:not(:disabled) {
  background-color: #c0392b;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Responsive styles for tablet */
@media (max-width: 1024px) {
  .data-table {
    font-size: 0.9rem;
  }

  .data-table th,
  .data-table td {
    padding: 0.75rem;
  }

  .btn {
    padding: 0.4rem 0.875rem;
    font-size: 0.8rem;
  }

  .status-badge {
    font-size: 0.8rem;
    padding: 0.2rem 0.6rem;
  }

  .sku-badge,
  .sku-count {
    font-size: 0.8rem;
    padding: 0.2rem 0.6rem;
  }

  .sku-list {
    gap: 0.4rem;
  }
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .table-container {
    -webkit-overflow-scrolling: touch;
  }

  .data-table {
    font-size: 0.85rem;
  }

  .data-table th,
  .data-table td {
    padding: 0.5rem;
    white-space: nowrap;
  }

  .actions {
    flex-direction: column;
    gap: 0.25rem;
    min-width: 100px;
  }

  .btn {
    width: 100%;
    padding: 0.4rem 0.75rem;
    font-size: 0.75rem;
  }

  .status-badge {
    font-size: 0.75rem;
    padding: 0.2rem 0.5rem;
  }

  .sku-badge,
  .sku-count {
    font-size: 0.75rem;
    padding: 0.2rem 0.5rem;
  }

  .sku-list {
    gap: 0.3rem;
    max-width: 200px;
  }

  .no-skus {
    font-size: 0.75rem;
  }
}
</style>
