<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Sku, CreateSkuDto, UpdateSkuDto } from '@/types/sku'
import type { Product } from '@/types/product'

interface Props {
  sku?: Sku | null
  mode: 'create' | 'edit'
  products: Product[]
  loading?: boolean
}

interface Emits {
  (e: 'submit', data: CreateSkuDto | UpdateSkuDto): void
  (e: 'cancel'): void
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})
const emit = defineEmits<Emits>()

const formData = ref({
  name: '',
  skuCode: '',
  productId: 0,
  description: ''
})

const errors = ref({
  name: '',
  skuCode: '',
  productId: '',
  description: ''
})

// Populate form when editing
watch(() => props.sku, (sku) => {
  if (sku) {
    formData.value = {
      name: sku.name,
      skuCode: sku.skuCode,
      productId: sku.productId,
      description: sku.description
    }
  } else {
    // Reset form for create mode
    formData.value = {
      name: '',
      skuCode: '',
      productId: props.products.length > 0 ? props.products[0]?.id ?? 0 : 0,
      description: ''
    }
  }
}, { immediate: true })

const validateForm = (): boolean => {
  let isValid = true
  errors.value = { name: '', skuCode: '', productId: '', description: '' }

  if (!formData.value.name.trim()) {
    errors.value.name = 'Name is required'
    isValid = false
  }

  if (!formData.value.skuCode.trim()) {
    errors.value.skuCode = 'SKU Code is required'
    isValid = false
  }

  if (!formData.value.productId || formData.value.productId === 0) {
    errors.value.productId = 'Product is required'
    isValid = false
  }

  if (!formData.value.description.trim()) {
    errors.value.description = 'Description is required'
    isValid = false
  }

  return isValid
}

const handleSubmit = () => {
  if (validateForm()) {
    emit('submit', formData.value)
  }
}

const handleCancel = () => {
  emit('cancel')
}
</script>

<template>
  <div class="form-overlay" @click.self="handleCancel">
    <div class="form-modal">
      <h3>{{ mode === 'create' ? 'Create SKU' : 'Edit SKU' }}</h3>
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="name">Name *</label>
          <input
            id="name"
            v-model="formData.name"
            type="text"
            :class="{ error: errors.name }"
            @input="errors.name = ''"
          />
          <span v-if="errors.name" class="error-message">{{ errors.name }}</span>
        </div>

        <div class="form-group">
          <label for="skuCode">SKU Code *</label>
          <input
            id="skuCode"
            v-model="formData.skuCode"
            type="text"
            :class="{ error: errors.skuCode }"
            @input="errors.skuCode = ''"
          />
          <span v-if="errors.skuCode" class="error-message">{{ errors.skuCode }}</span>
        </div>

        <div class="form-group">
          <label for="productId">Product *</label>
          <select
            id="productId"
            v-model.number="formData.productId"
            :class="{ error: errors.productId }"
            @change="errors.productId = ''"
          >
            <option :value="0" disabled>Select a product</option>
            <option v-for="product in products" :key="product.id" :value="product.id">
              {{ product.name }} ({{ product.productCode }})
            </option>
          </select>
          <span v-if="errors.productId" class="error-message">{{ errors.productId }}</span>
        </div>

        <div class="form-group">
          <label for="description">Description *</label>
          <textarea
            id="description"
            v-model="formData.description"
            rows="3"
            :class="{ error: errors.description }"
            @input="errors.description = ''"
          ></textarea>
          <span v-if="errors.description" class="error-message">{{ errors.description }}</span>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-cancel" @click="handleCancel" :disabled="loading">Cancel</button>
          <button type="submit" class="btn btn-submit" :disabled="loading">
            {{ loading ? 'Saving...' : (mode === 'create' ? 'Create' : 'Update') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.form-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.form-modal {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.form-modal h3 {
  margin: 0 0 1.5rem 0;
  color: #2c3e50;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #2c3e50;
  font-weight: 500;
}

.form-group input[type="text"],
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s;
  font-family: inherit;
}

.form-group input[type="text"]:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #00A3AD;
}

.form-group input.error,
.form-group select.error,
.form-group textarea.error {
  border-color: #e74c3c;
}

.form-group textarea {
  resize: vertical;
}

.error-message {
  display: block;
  color: #e74c3c;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-cancel {
  background-color: #95a5a6;
  color: white;
}

.btn-cancel:hover {
  background-color: #7f8c8d;
}

.btn-submit {
  background-color: #27ae60;
  color: white;
}

.btn-submit:hover:not(:disabled) {
  background-color: #229954;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .form-modal {
    width: 95%;
    padding: 1.5rem;
    max-height: 95vh;
  }

  .form-modal h3 {
    font-size: 1.25rem;
    margin-bottom: 1rem;
  }

  .form-group {
    margin-bottom: 1.25rem;
  }

  .form-group label {
    font-size: 0.9rem;
  }

  .form-group input[type="text"],
  .form-group select,
  .form-group textarea {
    padding: 0.625rem;
    font-size: 0.9rem;
  }

  .form-actions {
    flex-direction: column;
    gap: 0.75rem;
  }

  .btn {
    width: 100%;
    padding: 0.625rem 1rem;
    font-size: 0.9rem;
  }
}
</style>
