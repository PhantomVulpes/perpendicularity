import { ref, computed } from 'vue'
import type { RegisteredUser } from '@/api/apiclients/PerpendicularityApiClient'

const STORAGE_KEY = 'perpendicularity_user'

// Global reactive state
const currentUser = ref<RegisteredUser | null>(null)

// Initialize from localStorage on module load
const storedUser = localStorage.getItem(STORAGE_KEY)
if (storedUser) {
  try {
    currentUser.value = JSON.parse(storedUser) as RegisteredUser
  } catch (error) {
    console.error('Failed to parse stored user:', error)
    localStorage.removeItem(STORAGE_KEY)
  }
}

export function useAuth() {
  const isAuthenticated = computed(() => currentUser.value !== null)
  const user = computed(() => currentUser.value)

  const signIn = (user: RegisteredUser) => {
    currentUser.value = user
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user))
  }

  const signOut = () => {
    currentUser.value = null
    localStorage.removeItem(STORAGE_KEY)
  }

  return {
    user,
    isAuthenticated,
    signIn,
    signOut
  }
}
