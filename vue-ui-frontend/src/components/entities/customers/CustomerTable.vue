<script setup lang="ts">
import type { Customer } from '@/types/customer'

interface Props {
  customers: Customer[]
  loading: boolean
  operationLoading?: boolean
}

interface Emits {
  (e: 'edit', customer: Customer): void
  (e: 'delete', customer: Customer): void
}

withDefaults(defineProps<Props>(), {
  operationLoading: false
})
const emit = defineEmits<Emits>()

const handleEdit = (customer: Customer) => {
  emit('edit', customer)
}

const handleDelete = (customer: Customer) => {
  if (confirm(`Are you sure you want to delete customer "${customer.name}"?`)) {
    emit('delete', customer)
  }
}
</script>

<template>
  <div class="table-container">
    <div v-if="loading" class="loading">Loading customers...</div>
    <table v-else class="data-table">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Organization</th>
          <th>Visible</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="customers.length === 0">
          <td colspan="5" class="no-data">No customers found</td>
        </tr>
        <tr 
          v-for="customer in customers" 
          :key="customer.id"
          v-memo="[customer.id, customer.isVisible, operationLoading]"
        >
          <td>{{ customer.name }}</td>
          <td>{{ customer.email }}</td>
          <td>{{ customer.organization }}</td>
          <td>
            <span :class="['status-badge', customer.isVisible ? 'visible' : 'hidden']">
              {{ customer.isVisible ? 'Yes' : 'No' }}
            </span>
          </td>
          <td class="actions">
            <button class="btn btn-edit" @click="handleEdit(customer)" :disabled="operationLoading">Edit</button>
            <button class="btn btn-delete" @click="handleDelete(customer)" :disabled="operationLoading">Delete</button>
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
  min-width: 600px;
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

.status-badge.visible {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.hidden {
  background-color: #f8d7da;
  color: #721c24;
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
}
</style>
