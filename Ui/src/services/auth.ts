import { ref, computed } from 'vue'
import type { LoginResponse } from '@/api/apiclients/PerpendicularityApiClient'

const STORAGE_KEY = 'perpendicularity_auth'

/**
 * Checks if a JWT token is expired by decoding and checking the exp claim
 * @param token The JWT token to validate
 * @returns true if the token is expired or invalid, false otherwise
 */
function isTokenExpired(token: string): boolean {
  try {
    // JWT is in format: header.payload.signature
    const payload = JSON.parse(atob(token.split('.')[1]))
    
    // exp claim is in seconds, Date.now() is in milliseconds
    if (!payload.exp) {
      return true // No expiration claim means invalid token
    }
    
    return payload.exp * 1000 < Date.now()
  } catch (error) {
    console.error('Failed to decode JWT token:', error)
    return true // If we can't decode it, treat it as expired
  }
}

// Global reactive state
const authData = ref<LoginResponse | null>(null)

// Initialize from localStorage on module load
const storedAuth = localStorage.getItem(STORAGE_KEY)
if (storedAuth) {
  try {
    const parsedAuth = JSON.parse(storedAuth) as LoginResponse
    
    // Check if the stored token is expired
    if (parsedAuth.token && isTokenExpired(parsedAuth.token)) {
      console.log('Stored token is expired, clearing authentication')
      localStorage.removeItem(STORAGE_KEY)
    } else {
      authData.value = parsedAuth
    }
  } catch (error) {
    console.error('Failed to parse stored auth:', error)
    localStorage.removeItem(STORAGE_KEY)
  }
}

export function useAuth() {
  const isAuthenticated = computed(() => {
    if (!authData.value?.token) {
      return false
    }
    
    // Check if token is expired
    if (isTokenExpired(authData.value.token)) {
      // Auto-clear expired token
      authData.value = null
      localStorage.removeItem(STORAGE_KEY)
      return false
    }
    
    return true
  })
  const user = computed(() => authData.value ? {
    key: authData.value.userKey,
    firstName: authData.value.firstName,
    lastName: authData.value.lastName,
    displayName: authData.value.displayName,
    status: authData.value.status
  } : null)
  const token = computed(() => authData.value?.token ?? null)

  const signIn = (loginResponse: LoginResponse) => {
    authData.value = loginResponse
    localStorage.setItem(STORAGE_KEY, JSON.stringify(loginResponse))
  }

  const signOut = () => {
    authData.value = null
    localStorage.removeItem(STORAGE_KEY)
  }

  return {
    user,
    token,
    isAuthenticated,
    signIn,
    signOut
  }
}
