<script setup lang="ts">
import { ref } from 'vue'

interface Props {
  apiKeyValue: string
  apiKeyName: string
}

interface Emits {
  (e: 'close'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const copied = ref(false)

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(props.apiKeyValue)
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
</script>

<template>
  <div class="modal-overlay" @click.self="handleClose">
    <div class="modal-content">
      <div class="modal-header">
        <h3>API Key Created Successfully</h3>
      </div>
      
      <div class="modal-body">
        <div class="warning-box">
          <div class="warning-icon">⚠️</div>
          <div class="warning-text">
            <strong>Important:</strong> This is the only time you will see this API key. 
            Please copy it now and store it securely. You won't be able to retrieve it again.
          </div>
        </div>

        <div class="key-info">
          <label>API Key Name:</label>
          <div class="key-name">{{ apiKeyName }}</div>
        </div>

        <div class="key-display">
          <label>API Key:</label>
          <div class="key-value">
            <code>{{ apiKeyValue }}</code>
          </div>
        </div>

        <button class="btn btn-copy" @click="copyToClipboard">
          {{ copied ? '✓ Copied!' : 'Copy to Clipboard' }}
        </button>
      </div>

      <div class="modal-footer">
        <button class="btn btn-close" @click="handleClose">
          I've Saved the Key
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
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
  padding: 1.5rem 2rem;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.5rem;
}

.modal-body {
  padding: 2rem;
}

.warning-box {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background-color: #fff3cd;
  border: 1px solid #ffc107;
  border-radius: 4px;
  margin-bottom: 1.5rem;
}

.warning-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.warning-text {
  color: #856404;
  font-size: 0.95rem;
  line-height: 1.5;
}

.warning-text strong {
  display: block;
  margin-bottom: 0.25rem;
}

.key-info {
  margin-bottom: 1.5rem;
}

.key-info label {
  display: block;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.key-name {
  padding: 0.75rem;
  background-color: #f8f9fa;
  border-radius: 4px;
  color: #495057;
}

.key-display {
  margin-bottom: 1.5rem;
}

.key-display label {
  display: block;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.key-value {
  padding: 1rem;
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  word-break: break-all;
}

.key-value code {
  font-family: 'Courier New', Courier, monospace;
  font-size: 0.9rem;
  color: #e74c3c;
  font-weight: 600;
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
  margin-bottom: 1rem;
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
  background-color: #27ae60;
  color: white;
}

.btn-close:hover {
  background-color: #229954;
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

  .warning-box {
    flex-direction: column;
    align-items: center;
    text-align: center;
    padding: 0.75rem;
  }

  .key-display {
    font-size: 0.85rem;
    padding: 0.75rem;
  }

  .btn {
    width: 100%;
    padding: 0.625rem 1rem;
    font-size: 0.9rem;
  }
}
</style>
