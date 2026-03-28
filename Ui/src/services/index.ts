import { api } from './api'

/**
 * Example service for health check endpoint
 */
export const healthService = {
  /**
   * Check API health status
   */
  async checkHealth(): Promise<{ status: string; timestamp?: string }> {
    const { data, error } = await api.get<{ status: string; timestamp?: string }>('/health')
    
    if (error) {
      throw new Error(`Health check failed: ${error}`)
    }

    return data || { status: 'unknown' }
  },
}

// You can add more services here as your API grows
// Example:
// export const userService = {
//   async getUsers() { ... },
//   async createUser(user) { ... },
// }
