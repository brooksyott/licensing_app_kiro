<script setup lang="ts">
import { ref, watch, computed } from 'vue'
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
  customerId: '',
  productId: '',
  skuIds: [] as string[],
  rsaKeyId: '',
  licenseType: '',
  expirationDate: '',
  maxActivations: 1
})

const errors = ref({
  customerId: '',
  productId: '',
  skuIds: '',
  rsaKeyId: '',
  licenseType: '',
  expirationDate: '',
  maxActivations: ''
})

// Filter SKUs based on selected product
const filteredSkus = computed(() => {
  if (!formData.value.productId) {
    return []
  }
  return props.skus.filter(sku => sku.productId === formData.value.productId)
})

// Group SKUs by product for better UX (now filtered by selected product)
const skusByProduct = computed(() => {
  const grouped = new Map<string, { productId: string; productName: string; skus: Sku[] }>()
  
  filteredSkus.value.forEach(sku => {
    if (!grouped.has(sku.productId)) {
      grouped.set(sku.productId, {
        productId: sku.productId,
        productName: sku.productName,
        skus: []
      })
    }
    grouped.get(sku.productId)!.skus.push(sku)
  })
  
  return Array.from(grouped.values())
})

// Watch for product changes and clear SKU selection
watch(() => formData.value.productId, (newProductId, oldProductId) => {
  // Only clear SKUs if product actually changed and we're not in initial load
  if (oldProductId && newProductId !== oldProductId) {
    formData.value.skuIds = []
    errors.value.skuIds = ''
  }
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
      skuIds: license.skus.map(sku => sku.skuId),
      rsaKeyId: license.rsaKeyId,
      licenseType: license.licenseType,
      expirationDate: expirationDateValue,
      maxActivations: license.maxActivations
    }
  } else {
    // Reset form for create mode
    formData.value = {
      customerId: props.customers.length > 0 ? props.customers[0]?.id ?? '' : '',
      productId: props.products.length > 0 ? props.products[0]?.id ?? '' : '',
      skuIds: [],
      rsaKeyId: props.rsaKeys.length > 0 ? props.rsaKeys[0]?.id ?? '' : '',
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
    skuIds: '',
    rsaKeyId: '',
    licenseType: '',
    expirationDate: '',
    maxActivations: ''
  }

  if (!formData.value.customerId) {
    errors.value.customerId = 'Customer is required'
    isValid = false
  }

  if (!formData.value.productId) {
    errors.value.productId = 'Product is required'
    isValid = false
  }

  if (!formData.value.skuIds || formData.value.skuIds.length === 0) {
    errors.value.skuIds = 'At least one SKU must be selected'
    isValid = false
  }

  if (!formData.value.rsaKeyId) {
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

// Handle checkbox selection/deselection
const toggleSku = (skuId: string) => {
  const index = formData.value.skuIds.indexOf(skuId)
  if (index > -1) {
    formData.value.skuIds.splice(index, 1)
  } else {
    formData.value.skuIds.push(skuId)
  }
  // Clear error when user makes a selection
  if (formData.value.skuIds.length > 0) {
    errors.value.skuIds = ''
  }
}

const isSkuSelected = (skuId: string): boolean => {
  return formData.value.skuIds.includes(skuId)
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
            v-model="formData.customerId"
            :class="{ error: errors.customerId }"
            @change="errors.customerId = ''"
          >
            <option value="" disabled>Select a customer</option>
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
            v-model="formData.productId"
            :class="{ error: errors.productId }"
            @change="errors.productId = ''"
          >
            <option value="" disabled>Select a product</option>
            <option v-for="product in products" :key="product.id" :value="product.id">
              {{ product.name }} ({{ product.productCode }})
            </option>
          </select>
          <span v-if="errors.productId" class="error-message">{{ errors.productId }}</span>
        </div>

        <div class="form-group">
          <label>SKUs *</label>
          <div class="multi-select-container" :class="{ error: errors.skuIds }">
            <div v-if="!formData.productId" class="no-skus-message">
              Please select a product first to see available SKUs.
            </div>
            <div v-else-if="skusByProduct.length === 0" class="no-skus-message">
              No SKUs available for this product. Please create SKUs for this product first.
            </div>
            <div v-for="productGroup in skusByProduct" :key="productGroup.productId" class="product-group">
              <div class="product-header">{{ productGroup.productName }}</div>
              <div class="sku-list">
                <label
                  v-for="sku in productGroup.skus"
                  :key="sku.id"
                  class="sku-checkbox-label"
                >
                  <input
                    type="checkbox"
                    :id="`sku-${sku.id}`"
                    :value="sku.id"
                    :checked="isSkuSelected(sku.id)"
                    @change="toggleSku(sku.id)"
                    class="sku-checkbox"
                  />
                  <span class="sku-info">
                    <span class="sku-name">{{ sku.name }}</span>
                    <span class="sku-code">({{ sku.skuCode }})</span>
                  </span>
                </label>
              </div>
            </div>
          </div>
          <span v-if="errors.skuIds" class="error-message">{{ errors.skuIds }}</span>
        </div>

        <div class="form-group">
          <label for="rsaKeyId">RSA Key *</label>
          <select
            id="rsaKeyId"
            v-model="formData.rsaKeyId"
            :class="{ error: errors.rsaKeyId }"
            @change="errors.rsaKeyId = ''"
          >
            <option value="" disabled>Select an RSA key</option>
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
  border-color: #00A3AD;
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

.multi-select-container {
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 0.75rem;
  max-height: 300px;
  overflow-y: auto;
  background-color: #f9f9f9;
}

.multi-select-container.error {
  border-color: #e74c3c;
}

.no-skus-message {
  color: #7f8c8d;
  font-style: italic;
  text-align: center;
  padding: 1rem;
}

.product-group {
  margin-bottom: 1rem;
}

.product-group:last-child {
  margin-bottom: 0;
}

.product-header {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
  padding-bottom: 0.25rem;
  border-bottom: 2px solid #00A3AD;
  font-size: 0.95rem;
}

.sku-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  padding-left: 0.5rem;
}

.sku-checkbox-label {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.sku-checkbox-label:hover {
  background-color: #e8f4f8;
}

.sku-checkbox {
  margin-right: 0.75rem;
  cursor: pointer;
  width: 18px;
  height: 18px;
  flex-shrink: 0;
}

.sku-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.sku-name {
  color: #2c3e50;
  font-weight: 500;
}

.sku-code {
  color: #7f8c8d;
  font-size: 0.9rem;
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

  .multi-select-container {
    max-height: 200px;
    padding: 0.5rem;
  }

  .product-header {
    font-size: 0.875rem;
  }

  .sku-checkbox-label {
    padding: 0.375rem;
  }

  .sku-name {
    font-size: 0.875rem;
  }

  .sku-code {
    font-size: 0.8rem;
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
