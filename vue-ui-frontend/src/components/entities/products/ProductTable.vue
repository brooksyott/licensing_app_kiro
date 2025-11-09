<script setup lang="ts">
import type { Product } from '@/types/product'

interface Props {
  products: Product[]
  loading: boolean
  operationLoading?: boolean
}

interface Emits {
  (e: 'edit', product: Product): void
  (e: 'delete', product: Product): void
}

withDefaults(defineProps<Props>(), {
  operationLoading: false
})
const emit = defineEmits<Emits>()

const handleEdit = (product: Product) => {
  emit('edit', product)
}

const handleDelete = (product: Product) => {
  if (confirm(`Are you sure you want to delete product "${product.name}"?`)) {
    emit('delete', product)
  }
}
</script>

<template>
  <div class="table-container">
    <div v-if="loading" class="loading">Loading products...</div>
    <table v-else class="data-table">
      <thead>
        <tr>
          <th>Name</th>
          <th>Product Code</th>
          <th>Description</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="products.length === 0">
          <td colspan="4" class="no-data">No products found</td>
        </tr>
        <tr 
          v-for="product in products" 
          :key="product.id"
          v-memo="[product.id, operationLoading]"
        >
          <td>{{ product.name }}</td>
          <td>{{ product.productCode }}</td>
          <td>{{ product.description }}</td>
          <td class="actions">
            <button class="btn btn-edit" @click="handleEdit(product)" :disabled="operationLoading">Edit</button>
            <button class="btn btn-delete" @click="handleDelete(product)" :disabled="operationLoading">Delete</button>
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
}
</style>
