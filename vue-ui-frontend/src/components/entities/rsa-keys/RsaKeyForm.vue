<script setup lang="ts">
import { ref } from 'vue'
import type { CreateRsaKeyDto } from '@/types/rsaKey'

interface Props {
  loading?: boolean
}

interface Emits {
  (e: 'submit', data: CreateRsaKeyDto): void
  (e: 'cancel'): void
}

withDefaults(defineProps<Props>(), {
  loading: false
})

const emit = defineEmits<Emits>()

const formData = ref({
  name: '',
  keySize: 2048,
  createdBy: ''
})

const errors = ref({
  name: '',
  createdBy: ''
})

const keySizeOptions = [2048, 3072, 4096]

const validateForm = (): boolean => {
  let isValid = true
  errors.value = { name: '', createdBy: '' }

  if (!formData.value.name.trim()) {
    errors.value.name = 'Name is required'
    isValid = false
  }

  if (!formData.value.createdBy.trim()) {
    errors.value.createdBy = 'Created By is required'
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
      <h3>Create RSA Key</h3>
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
          <label for="keySize">Key Size *</label>
          <select
            id="keySize"
            v-model.number="formData.keySize"
          >
            <option v-for="size in keySizeOptions" :key="size" :value="size">
              {{ size }} bits
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="createdBy">Created By *</label>
          <input
            id="createdBy"
            v-model="formData.createdBy"
            type="text"
            :class="{ error: errors.createdBy }"
            @input="errors.createdBy = ''"
          />
          <span v-if="errors.createdBy" class="error-message">{{ errors.createdBy }}</span>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-cancel" @click="handleCancel" :disabled="loading">Cancel</button>
          <button type="submit" class="btn btn-submit" :disabled="loading">
            {{ loading ? 'Creating...' : 'Create' }}
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
.form-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-group input[type="text"]:focus,
.form-group select:focus {
  outline: none;
  border-color: #3498db;
}

.form-group input.error {
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
