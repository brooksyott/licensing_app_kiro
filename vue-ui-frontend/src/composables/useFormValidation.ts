import { ref } from 'vue'

export interface ValidationRule {
  required?: boolean
  email?: boolean
  minLength?: number
  maxLength?: number
  pattern?: RegExp
  custom?: (value: any) => boolean
  message: string
}

export interface ValidationRules {
  [key: string]: ValidationRule[]
}

export function useFormValidation() {
  const errors = ref<Record<string, string>>({})

  const validateField = (fieldName: string, value: any, rules: ValidationRule[]): boolean => {
    for (const rule of rules) {
      if (rule.required && !value) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.email && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(value)) {
          errors.value[fieldName] = rule.message
          return false
        }
      }

      if (rule.minLength && value && value.length < rule.minLength) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.maxLength && value && value.length > rule.maxLength) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.pattern && value && !rule.pattern.test(value)) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.custom && !rule.custom(value)) {
        errors.value[fieldName] = rule.message
        return false
      }
    }

    errors.value[fieldName] = ''
    return true
  }

  const validateForm = (formData: Record<string, any>, rules: ValidationRules): boolean => {
    let isValid = true
    errors.value = {}

    for (const [fieldName, fieldRules] of Object.entries(rules)) {
      if (!validateField(fieldName, formData[fieldName], fieldRules)) {
        isValid = false
      }
    }

    return isValid
  }

  const clearError = (fieldName: string) => {
    errors.value[fieldName] = ''
  }

  const clearAllErrors = () => {
    errors.value = {}
  }

  return {
    errors,
    validateField,
    validateForm,
    clearError,
    clearAllErrors
  }
}
