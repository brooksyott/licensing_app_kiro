import { AxiosError } from 'axios'

/**
 * Error Handler Utility
 * 
 * This module provides centralized error handling for API requests.
 * It converts technical HTTP errors into user-friendly messages that
 * provide clear guidance on what went wrong and how to resolve it.
 * 
 * Usage:
 * - Import getErrorMessage in axios interceptors
 * - Pass AxiosError to get a formatted user-friendly message
 * - Display the message to users via notifications
 * 
 * Supported error codes:
 * - 400: Bad Request (with validation error extraction)
 * - 401: Unauthorized
 * - 403: Forbidden
 * - 404: Not Found
 * - 409: Conflict
 * - 422: Unprocessable Entity (validation errors)
 * - 500: Internal Server Error
 * - 502: Bad Gateway
 * - 503: Service Unavailable
 * - 504: Gateway Timeout
 * - Network errors (ECONNABORTED, ERR_NETWORK, ETIMEDOUT)
 */

/**
 * Error response structure from the API
 */
interface ApiErrorResponse {
  message?: string
  title?: string
  errors?: Record<string, string[]>
  [key: string]: any
}

/**
 * Extracts a user-friendly error message from an Axios error
 * Handles common HTTP status codes and provides clear, actionable messages
 * 
 * @param error - The Axios error object from a failed request
 * @returns A user-friendly error message string
 */
export function getErrorMessage(error: AxiosError<ApiErrorResponse>): string {
  const status = error.response?.status
  const data = error.response?.data
  
  // Handle specific HTTP status codes
  switch (status) {
    case 400:
      return handleBadRequest(data)
    
    case 401:
      return 'Authentication required. Please log in with a valid API key.'
    
    case 403:
      return 'Access denied. You do not have permission to perform this action.'
    
    case 404:
      return 'The requested resource was not found. It may have been deleted or moved.'
    
    case 409:
      return handleConflict(data)
    
    case 422:
      return handleValidationError(data)
    
    case 500:
      return 'A server error occurred. Please try again later or contact support if the problem persists.'
    
    case 502:
      return 'The server is temporarily unavailable. Please try again in a few moments.'
    
    case 503:
      return 'The service is currently unavailable. Please try again later.'
    
    case 504:
      return 'The request timed out. Please check your connection and try again.'
    
    default:
      return handleGenericError(error, data)
  }
}

/**
 * Handles 400 Bad Request errors
 */
function handleBadRequest(data?: ApiErrorResponse): string {
  if (!data) {
    return 'Invalid request. Please check your input and try again.'
  }
  
  // Check for validation errors
  if (data.errors && Object.keys(data.errors).length > 0) {
    const validationErrors = Object.entries(data.errors)
      .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
      .join('; ')
    return `Invalid input - ${validationErrors}`
  }
  
  // Check for message in response
  if (data.message) {
    return `Bad request: ${data.message}`
  }
  
  if (data.title) {
    return `Bad request: ${data.title}`
  }
  
  return 'Invalid request. Please check your input and try again.'
}

/**
 * Handles 409 Conflict errors (typically duplicate entries)
 */
function handleConflict(data?: ApiErrorResponse): string {
  if (!data) {
    return 'A conflict occurred. This resource may already exist.'
  }
  
  if (data.message) {
    return data.message
  }
  
  if (data.title) {
    return data.title
  }
  
  return 'A conflict occurred. This resource may already exist or is in use.'
}

/**
 * Handles 422 Unprocessable Entity errors (validation failures)
 */
function handleValidationError(data?: ApiErrorResponse): string {
  if (!data) {
    return 'The data provided could not be processed. Please check your input.'
  }
  
  // Check for validation errors
  if (data.errors && Object.keys(data.errors).length > 0) {
    const validationErrors = Object.entries(data.errors)
      .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
      .join('; ')
    return `Validation failed - ${validationErrors}`
  }
  
  if (data.message) {
    return `Validation failed: ${data.message}`
  }
  
  return 'The data provided could not be processed. Please check your input.'
}

/**
 * Handles generic errors and network issues
 */
function handleGenericError(error: AxiosError, data?: ApiErrorResponse): string {
  // Handle network errors
  if (error.code === 'ECONNABORTED') {
    return 'The request took too long to complete. Please try again.'
  }
  
  if (error.code === 'ERR_NETWORK') {
    return 'Network error. Please check your internet connection and try again.'
  }
  
  if (error.code === 'ETIMEDOUT') {
    return 'Connection timed out. Please check your internet connection and try again.'
  }
  
  // Try to extract message from response data
  if (data) {
    if (typeof data === 'string') {
      return data
    }
    
    if (data.message) {
      return data.message
    }
    
    if (data.title) {
      return data.title
    }
  }
  
  // Fallback to error message
  if (error.message) {
    return `An error occurred: ${error.message}`
  }
  
  return 'An unexpected error occurred. Please try again.'
}

/**
 * Formats validation errors from API response into a readable string
 */
export function formatValidationErrors(errors: Record<string, string[]>): string {
  return Object.entries(errors)
    .map(([field, messages]) => {
      const fieldName = field.charAt(0).toUpperCase() + field.slice(1)
      return `${fieldName}: ${messages.join(', ')}`
    })
    .join('\n')
}
