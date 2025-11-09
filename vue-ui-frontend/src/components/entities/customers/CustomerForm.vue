<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer'

interface Props {
  customer?: Customer | null
  mode: 'create' | 'edit'
  loading?: boolean
}

interface Emits {
  (e: 'submit', data: CreateCustomerDto | UpdateCustomerDto): void
  (e: 'cancel'): void
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})
const emit = defineEmits<Emits>()

const formData = ref({
  name: '',
  email: '',
  organization: '',
  isVisible: true
})

const errors = ref({
  name: '',
  email: '',
  organization: ''
})

// Populate form when editing
watch(() => props.customer, (customer) => {
  if (customer) {
    formData.value = {
      name: customer.name,
      email: customer.email,
      organization: customer.organization,
      isVisible: customer.isVisible
    }
  } else {
    // Reset form for create mode
    formData.value = {
      name: '',
      email: '',
      organization: '',
      isVisible: true
    }
  }
}, { immediate: true })

const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

const validateForm = (): boolean => {
  let isValid = true
  errors.value = { name: '', email: '', organization: '' }

  if (!formData.value.name.trim()) {
    errors.value.name = 'Name is required'
    isValid = false
  }

  if (!formData.value.email.trim()) {
    errors.value.email = 'Email is required'
    isValid = false
  } else if (!validateEmail(formData.value.email)) {
    errors.value.email = 'Invalid email format'
    isValid = false
  }

  if (!formData.value.organization.trim()) {
    errors.value.organization = 'Organization is required'
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
      <h3>{{ mode === 'create' ? 'Create Customer' : 'Edit Customer' }}</h3>
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
          <label for="email">Email *</label>
          <input
            id="email"
            v-model="formData.email"
            type="email"
            :class="{ error: errors.email }"
            @input="errors.email = ''"
          />
          <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
        </div>

        <div class="form-group">
          <label for="organization">Organization *</label>
          <input
            id="organization"
            v-model="formData.organization"
            type="text"
            :class="{ error: errors.organization }"
            @input="errors.organization = ''"
          />
          <span v-if="errors.organization" class="error-message">{{ errors.organization }}</span>
        </div>

        <div class="form-group checkbox-group">
          <label>
            <input v-model="formData.isVisible" type="checkbox" />
            <span>Visible</span>
          </label>
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
.form-group input[type="email"] {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-group input[type="text"]:focus,
.form-group input[type="email"]:focus {
  outline: none;
  border-color: #00A3AD;
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

.checkbox-group label {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.checkbox-group input[type="checkbox"] {
  margin-right: 0.5rem;
  cursor: pointer;
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
  .form-group input[type="email"] {
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
