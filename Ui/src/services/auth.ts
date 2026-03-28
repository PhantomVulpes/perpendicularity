import { ref, computed } from 'vue'
import type { LoginResponse } from '@/api/apiclients/PerpendicularityApiClient'

const STORAGE_KEY = 'perpendicularity_auth'

// Global reactive state
const authData = ref<LoginResponse | null>(null)

// Initialize from localStorage on module load
const storedAuth = localStorage.getItem(STORAGE_KEY)
if (storedAuth) {
  try {
    authData.value = JSON.parse(storedAuth) as LoginResponse
  } catch (error) {
    console.error('Failed to parse stored auth:', error)
    localStorage.removeItem(STORAGE_KEY)
  }
}

export function useAuth() {
  const isAuthenticated = computed(() => authData.value !== null)
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
