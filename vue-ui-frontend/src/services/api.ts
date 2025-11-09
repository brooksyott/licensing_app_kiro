import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { useAuth } from '@/composables/useAuth'
import { getErrorMessage } from '@/utils/errorHandler'

interface ApiErrorResponse {
  message?: string
  title?: string
  errors?: Record<string, string[]>
  [key: string]: any
}

// Create axios instance with base configuration
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5264',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add Auth_Key header
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const { apiKey } = useAuth()
    
    // Add Auth_Key header if user is logged in
    if (apiKey.value) {
      config.headers.set('Auth_Key', apiKey.value)
    }
    
    return config
  },
  (error: AxiosError) => {
    return Promise.reject(error)
  }
)

// Response interceptor for global error handling
api.interceptors.response.use(
  (response) => {
    // Pass through successful responses
    return response
  },
  (error: unknown) => {
    const axiosError = error as AxiosError<ApiErrorResponse>
    
    // Get user-friendly error message
    const errorMessage = getErrorMessage(axiosError)
    
    // Dispatch custom event for API errors with formatted message
    const errorEvent = new CustomEvent('api-error', {
      detail: {
        status: axiosError.response?.status,
        message: errorMessage,
        originalError: error
      }
    })
    window.dispatchEvent(errorEvent)
    
    return Promise.reject(error)
  }
)

export default api
