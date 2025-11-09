import { ref, readonly } from 'vue'

// Shared authentication state (defined outside the composable to ensure it's shared across all instances)
const isLoggedIn = ref(true)
const apiKey = ref<string | null>('Dg0gkEdvsWyyLbviE2KjZKrN5g1wZeSW')

/**
 * Composable for managing authentication state across the application.
 * Provides shared state and methods for login/logout operations.
 */
export function useAuth() {
  /**
   * Logs the user in and sets a sample API key
   */
  const login = () => {
    isLoggedIn.value = true
    // In a real implementation, this would be set from actual authentication
    apiKey.value = 'Dg0gkEdvsWyyLbviE2KjZKrN5g1wZeSW'
  }

  /**
   * Logs the user out and clears the API key
   */
  const logout = () => {
    isLoggedIn.value = false
    apiKey.value = null
  }

  /**
   * Toggles between logged in and logged out states
   */
  const toggleAuth = () => {
    if (isLoggedIn.value) {
      logout()
    } else {
      login()
    }
  }

  // Return readonly wrappers to prevent external mutation
  return {
    isLoggedIn: readonly(isLoggedIn),
    apiKey: readonly(apiKey),
    login,
    logout,
    toggleAuth
  }
}
