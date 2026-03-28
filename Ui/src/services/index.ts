import { Client } from '@/api/apiclients/PerpendicularityApiClient'

/**
 * Example service for health check endpoint
 */
export const healthService = {
  /**
   * Check API health status
   */
  async checkHealth(): Promise<{ status: string }> {
    try {
      const client = new Client('http://localhost:63000')
      await client.health()
      return { status: 'healthy' }
    } catch (error) {
      console.error('Health check failed:', error)
      throw new Error(`Health check failed: ${error}`)
    }
  },
}

// You can add more services here as your API grows
// Example:
// export const userService = {
//   async getUsers() { ... },
//   async createUser(user) { ... },
// }
