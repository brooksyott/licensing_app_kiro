<script setup lang="ts">
import { ref } from 'vue'
import type { License } from '@/types/license'

interface Props {
  license: License
}

interface Emits {
  (e: 'close'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const copied = ref(false)

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(props.license.signedPayload)
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 2000)
  } catch (err) {
    console.error('Failed to copy:', err)
  }
}

const handleClose = () => {
  emit('close')
}

const formatDate = (date: string | null) => {
  if (!date) return 'N/A'
  return new Date(date).toLocaleDateString()
}
</script>

<template>
  <div class="modal-overlay" @click.self="handleClose">
    <div class="modal-content">
      <div class="modal-header">
        <h3>License Details</h3>
        <button class="close-button" @click="handleClose">×</button>
      </div>
      
      <div class="modal-body">
        <div class="license-info-grid">
          <div class="info-item">
            <label>Customer:</label>
            <div class="info-value">{{ license.customerName }}</div>
          </div>

          <div class="info-item">
            <label>Product:</label>
            <div class="info-value">{{ license.productName }}</div>
          </div>

          <div class="info-item">
            <label>License Type:</label>
            <div class="info-value">{{ license.licenseType }}</div>
          </div>

          <div class="info-item">
            <label>Status:</label>
            <div class="info-value">
              <span :class="['status-badge', license.status.toLowerCase()]">
                {{ license.status }}
              </span>
            </div>
          </div>

          <div class="info-item">
            <label>Expiration Date:</label>
            <div class="info-value">{{ formatDate(license.expirationDate) }}</div>
          </div>

          <div class="info-item">
            <label>Activations:</label>
            <div class="info-value">{{ license.currentActivations }} / {{ license.maxActivations }}</div>
          </div>

          <div class="info-item">
            <label>RSA Key:</label>
            <div class="info-value">{{ license.rsaKeyName }}</div>
          </div>

          <div class="info-item full-width">
            <label>SKUs:</label>
            <div class="sku-list">
              <span 
                v-for="sku in license.skus" 
                :key="sku.skuId"
                class="sku-badge"
              >
                {{ sku.skuName }} ({{ sku.skuCode }})
              </span>
              <span v-if="license.skus.length === 0" class="no-skus">
                No SKUs assigned
              </span>
            </div>
          </div>
        </div>

        <div class="license-key-section">
          <label>License:</label>
          <div class="key-value">
            <code>{{ license.signedPayload }}</code>
          </div>
          <button class="btn btn-copy" @click="copyToClipboard">
            {{ copied ? '✓ Copied!' : 'Copy License' }}
          </button>
        </div>
      </div>

      <div class="modal-footer">
        <button class="btn btn-close" @click="handleClose">
          Close
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
  padding: 1.5rem 2rem;
  border-bottom: 1px solid #e9ecef;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.5rem;
}

.close-button {
  background: none;
  border: none;
  font-size: 2rem;
  color: #7f8c8d;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s;
}

.close-button:hover {
  background-color: #f8f9fa;
  color: #2c3e50;
}

.modal-body {
  padding: 2rem;
}

.license-info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.info-item {
  display: flex;
  flex-direction: column;
}

.info-item.full-width {
  grid-column: 1 / -1;
}

.info-item label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
  font-size: 0.9rem;
}

.info-value {
  padding: 0.75rem;
  background-color: #f8f9fa;
  border-radius: 4px;
  color: #495057;
}

.status-badge {
  display: inline-block;
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
  padding: 0.75rem;
  background-color: #f8f9fa;
  border-radius: 4px;
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

.no-skus {
  color: #9e9e9e;
  font-style: italic;
  font-size: 0.85rem;
}

.license-key-section {
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 2px solid #e9ecef;
}

.license-key-section label {
  display: block;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.75rem;
  font-size: 1rem;
}

.key-value {
  padding: 1rem;
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  word-break: break-all;
  margin-bottom: 1rem;
  max-height: 200px;
  overflow-y: auto;
}

.key-value code {
  font-family: 'Courier New', Courier, monospace;
  font-size: 0.85rem;
  color: #e74c3c;
  font-weight: 600;
  line-height: 1.5;
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

.btn-copy {
  width: 100%;
  background-color: #00A3AD;
  color: white;
}

.btn-copy:hover {
  background-color: #008A93;
}

.modal-footer {
  padding: 1.5rem 2rem;
  border-top: 1px solid #e9ecef;
  display: flex;
  justify-content: flex-end;
}

.btn-close {
  background-color: #7f8c8d;
  color: white;
}

.btn-close:hover {
  background-color: #6c7a89;
}

/* Responsive styles for tablet */
@media (max-width: 1024px) {
  .license-info-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .info-item.full-width {
    grid-column: 1;
  }
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .modal-content {
    width: 95%;
    max-height: 95vh;
  }

  .modal-header,
  .modal-body,
  .modal-footer {
    padding: 1rem;
  }

  .modal-header h3 {
    font-size: 1.25rem;
  }

  .license-info-grid {
    gap: 1rem;
  }

  .key-value {
    font-size: 0.75rem;
    padding: 0.75rem;
  }

  .btn {
    width: 100%;
    padding: 0.625rem 1rem;
    font-size: 0.9rem;
  }
}
</style>
