<script setup lang="ts">
import { ref, watch } from 'vue'
import type { License, CreateLicenseDto, UpdateLicenseDto } from '@/types/license'
import type { Customer } from '@/types/customer'
import type { Product } from '@/types/product'
import type { Sku } from '@/types/sku'
import type { RsaKey } from '@/types/rsaKey'

interface Props {
  license?: License | null
  mode: 'create' | 'edit'
  customers: Customer[]
  products: Product[]
  skus: Sku[]
  rsaKeys: RsaKey[]
  loading?: boolean
}

interface Emits {
  (e: 'submit', data: CreateLicenseDto | UpdateLicenseDto): void
  (e: 'cancel'): void
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})
const emit = defineEmits<Emits>()

const formData = ref({
  customerId: 0,
  productId: 0,
  skuId: 0,
  rsaKeyId: 0,
  licenseType: '',
  expirationDate: '',
  maxActivations: 1
})

const errors = ref({
  customerId: '',
  productId: '',
  skuId: '',
  rsaKeyId: '',
  licenseType: '',
  expirationDate: '',
  maxActivations: ''
})

// Populate form when editing
watch(() => props.license, (license) => {
  if (license) {
    let expirationDateValue = ''
    if (license.expirationDate) {
      const parts = license.expirationDate.split('T')
      expirationDateValue = parts[0] || ''
    }
    
    formData.value = {
      customerId: license.customerId,
      productId: license.productId,
      skuId: license.skuId,
      rsaKeyId: license.rsaKeyId,
      licenseType: license.licenseType,
      expirationDate: expirationDateValue,
      maxActivations: license.maxActivations
    }
  } else {
    // Reset form for create mode
    formData.value = {
      customerId: props.customers.length > 0 ? props.customers[0]?.id ?? 0 : 0,
      productId: props.products.length > 0 ? props.products[0]?.id ?? 0 : 0,
      skuId: props.skus.length > 0 ? props.skus[0]?.id ?? 0 : 0,
      rsaKeyId: props.rsaKeys.length > 0 ? props.rsaKeys[0]?.id ?? 0 : 0,
      licenseType: '',
      expirationDate: '',
      maxActivations: 1
    }
  }
}, { immediate: true })

const validateForm = (): boolean => {
  let isValid = true
  errors.value = {
    customerId: '',
    productId: '',
    skuId: '',
    rsaKeyId: '',
    licenseType: '',
    expirationDate: '',
    maxActivations: ''
  }

  if (!formData.value.customerId || formData.value.customerId === 0) {
    errors.value.customerId = 'Customer is required'
    isValid = false
  }

  if (!formData.value.productId || formData.value.productId === 0) {
    errors.value.productId = 'Product is required'
    isValid = false
  }

  if (!formData.value.skuId || formData.value.skuId === 0) {
    errors.value.skuId = 'SKU is required'
    isValid = false
  }

  if (!formData.value.rsaKeyId || formData.value.rsaKeyId === 0) {
    errors.value.rsaKeyId = 'RSA Key is required'
    isValid = false
  }

  if (!formData.value.licenseType.trim()) {
    errors.value.licenseType = 'License Type is required'
    isValid = false
  }

  if (formData.value.maxActivations < 1) {
    errors.value.maxActivations = 'Max Activations must be at least 1'
    isValid = false
  }

  return isValid
}

const handleSubmit = () => {
  if (validateForm()) {
    const submitData = {
      ...formData.value,
      expirationDate: formData.value.expirationDate || null
    }
    emit('submit', submitData)
  }
}

const handleCancel = () => {
  emit('cancel')
}
</script>

<template>
  <div class="form-overlay" @click.self="handleCancel">
    <div class="form-modal">
      <h3>{{ mode === 'create' ? 'Create License' : 'Edit License' }}</h3>
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="customerId">Customer *</label>
          <select
            id="customerId"
            v-model.number="formData.customerId"
            :class="{ error: errors.customerId }"
            @change="errors.customerId = ''"
          >
            <option :value="0" disabled>Select a customer</option>
            <option v-for="customer in customers" :key="customer.id" :value="customer.id">
              {{ customer.name }} ({{ customer.email }})
            </option>
          </select>
          <span v-if="errors.customerId" class="error-message">{{ errors.customerId }}</span>
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
          <label for="skuId">SKU *</label>
          <select
            id="skuId"
            v-model.number="formData.skuId"
            :class="{ error: errors.skuId }"
            @change="errors.skuId = ''"
          >
            <option :value="0" disabled>Select a SKU</option>
            <option v-for="sku in skus" :key="sku.id" :value="sku.id">
              {{ sku.name }} ({{ sku.skuCode }})
            </option>
          </select>
          <span v-if="errors.skuId" class="error-message">{{ errors.skuId }}</span>
        </div>

        <div class="form-group">
          <label for="rsaKeyId">RSA Key *</label>
          <select
            id="rsaKeyId"
            v-model.number="formData.rsaKeyId"
            :class="{ error: errors.rsaKeyId }"
            @change="errors.rsaKeyId = ''"
          >
            <option :value="0" disabled>Select an RSA key</option>
            <option v-for="rsaKey in rsaKeys" :key="rsaKey.id" :value="rsaKey.id">
              {{ rsaKey.name }} ({{ rsaKey.keySize }} bits)
            </option>
          </select>
          <span v-if="errors.rsaKeyId" class="error-message">{{ errors.rsaKeyId }}</span>
        </div>

        <div class="form-group">
          <label for="licenseType">License Type *</label>
          <input
            id="licenseType"
            v-model="formData.licenseType"
            type="text"
            :class="{ error: errors.licenseType }"
            @input="errors.licenseType = ''"
            placeholder="e.g., Trial, Standard, Enterprise"
          />
          <span v-if="errors.licenseType" class="error-message">{{ errors.licenseType }}</span>
        </div>

        <div class="form-group">
          <label for="expirationDate">Expiration Date</label>
          <input
            id="expirationDate"
            v-model="formData.expirationDate"
            type="date"
            :class="{ error: errors.expirationDate }"
            @input="errors.expirationDate = ''"
          />
          <span v-if="errors.expirationDate" class="error-message">{{ errors.expirationDate }}</span>
        </div>

        <div class="form-group">
          <label for="maxActivations">Max Activations *</label>
          <input
            id="maxActivations"
            v-model.number="formData.maxActivations"
            type="number"
            min="1"
            :class="{ error: errors.maxActivations }"
            @input="errors.maxActivations = ''"
          />
          <span v-if="errors.maxActivations" class="error-message">{{ errors.maxActivations }}</span>
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
  max-width: 600px;
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
.form-group input[type="date"],
.form-group input[type="number"],
.form-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s;
  font-family: inherit;
}

.form-group input[type="text"]:focus,
.form-group input[type="date"]:focus,
.form-group input[type="number"]:focus,
.form-group select:focus {
  outline: none;
  border-color: #3498db;
}

.form-group input.error,
.form-group select.error {
  border-color: #e74c3c;
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
  .form-group input[type="number"],
  .form-group input[type="date"],
  .form-group select {
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
