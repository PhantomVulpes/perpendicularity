import { Client } from './apiclients/Perpendicularity/PerpendicularityApiClient'
import { useAuth } from '@/services/auth'

// Use empty string to make requests relative to current origin
// This allows Vite's proxy to handle the requests in development
const API_BASE_URL = ''

/**
 * Creates an authenticated API client with the current user's token
 */
export function createAuthenticatedClient(): Client {
  const { token } = useAuth()
  
  // Create a custom fetch wrapper that adds the Authorization header
  const authenticatedFetch = {
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
      const headers = new Headers(init?.headers)
      
      // Add Authorization header if token exists
      if (token.value) {
        headers.set('Authorization', `Bearer ${token.value}`)
      }
      
      const updatedInit: RequestInit = {
        ...init,
        headers
      }
      
      return window.fetch(url, updatedInit)
    }
  }
  
  return new Client(API_BASE_URL, authenticatedFetch)
}

/**
 * Creates an unauthenticated API client (for login, register, etc.)
 */
export function createClient(): Client {
  return new Client(API_BASE_URL)
}
